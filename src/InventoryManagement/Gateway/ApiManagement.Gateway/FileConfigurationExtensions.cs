using Ocelot.Configuration.File;



namespace Gateway
{
    public class GlobalHosts : Dictionary<string, Uri> { }
    public static class FileConfigurationExtensions
    {
        public static IServiceCollection ConfigureDownstreamHostAndPortsPlaceholders(this IServiceCollection services, IConfiguration configuration)
        {
            services.PostConfigure<FileConfiguration>(fileConfiguration =>
            {
                var globalHosts = configuration.GetSection($"{nameof(FileConfiguration.GlobalConfiguration)}:Hosts").Get<GlobalHosts>();

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureRote(route, globalHosts);
                }
            });
            return services;
        }
        private static void ConfigureRote(FileRoute route, GlobalHosts globalHosts)
        {
            foreach (var hostAndPort in route.DownstreamHostAndPorts)
            {
                var host = hostAndPort.Host;
                if (host.StartsWith("{") && host.EndsWith("}"))
                {
                    var placeHolder = host.TrimStart('{').TrimEnd('}');
                    if (globalHosts.TryGetValue(placeHolder, out var uri))
                    {
                        route.DownstreamScheme = uri.Scheme;
                        hostAndPort.Host = uri.Host;
                        hostAndPort.Port = uri.Port;
                    }
                }
            }
            route.DownstreamPathTemplate = ReplacePlaceholders(route.DownstreamPathTemplate, globalHosts);
        }
        private static string ReplacePlaceholders(string downstreamPathTemplate, GlobalHosts globalHosts)
        {
            var placeholders = GetPlaceholders(downstreamPathTemplate);

            foreach (var placeholder in placeholders)
            {
                var placeholderValue = globalHosts.GetValueOrDefault(placeholder);
                if (placeholderValue != null)
                {
                    downstreamPathTemplate = downstreamPathTemplate.Replace("{" + placeholder + "}", placeholderValue.ToString());
                }
            }
            return downstreamPathTemplate;
        }

        private static List<string> GetPlaceholders(string downstreamPathTemplate)
        {
            var placeholders = new List<string>();

            var startIndex = downstreamPathTemplate.IndexOf("{");
            var endIndex = downstreamPathTemplate.IndexOf("}");

            while (startIndex != -1 && endIndex != -1)
            {
                var placeholder = downstreamPathTemplate.Substring(startIndex + 1, endIndex - startIndex - 1);
                placeholders.Add(placeholder);

                startIndex = downstreamPathTemplate.IndexOf("{", endIndex);
                endIndex = downstreamPathTemplate.IndexOf("}", endIndex + 1);
            }

            return placeholders;
        }
    }
}
