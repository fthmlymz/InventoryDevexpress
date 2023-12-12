using System.Text;

namespace ApiManagement.Gateway
{
    public class KeycloakFormContentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public KeycloakFormContentMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/gateway/inventory/user/token/getToken") && context.Request.Method == "POST")
            {
                var formContent = await context.Request.ReadFormAsync();
                var formFields = new Dictionary<string, string>();

                foreach (var field in formContent)
                {
                    formFields[field.Key] = field.Value;
                }

                formFields["grant_type"] = _configuration.GetSection("Keycloak:GrantType").Value;
                formFields["client_id"] = _configuration.GetSection("Keycloak:ClientId").Value;
                formFields["client_secret"] = _configuration.GetSection("Keycloak:ClientSecret").Value;

                var formData = FormatFormData(formFields);

                context.Request.Body = new MemoryStream(formData);
                context.Request.ContentLength = formData.Length;
                context.Request.ContentType = "application/x-www-form-urlencoded";
            }
            else if (context.Request.Path.StartsWithSegments("/gateway/inventory/user/token/permissions") && context.Request.Method == "POST")
            {
                var formContent = await context.Request.ReadFormAsync();
                var formFields = new Dictionary<string, string>();
                foreach (var field in formContent)
                {
                    formFields[field.Key] = field.Value;
                }
                formFields["grant_type"] = _configuration.GetSection("Keycloak:GrantTypePermissions").Value;
                formFields["client_id"] = _configuration.GetSection("Keycloak:ClientId").Value;
                formFields["client_secret"] = _configuration.GetSection("Keycloak:ClientSecret").Value;
                formFields["realm"] = _configuration.GetSection("Keycloak:Realm").Value;
                formFields["audience"] = _configuration.GetSection("Keycloak:Audience").Value;

                var formData = FormatFormData(formFields);
                context.Request.Body = new MemoryStream(formData);
                context.Request.ContentLength = formData.Length;
                context.Request.ContentType = "application/x-www-form-urlencoded";
            }
            else if (context.Request.Path.StartsWithSegments("/gateway/inventory/user/token/refreshToken") && context.Request.Method == "POST")
            {
                var formContent = await context.Request.ReadFormAsync();
                var formFields = new Dictionary<string, string>();
                foreach (var field in formContent)
                {
                    formFields[field.Key] = field.Value;
                }
                formFields["grant_type"] = "refresh_token";
                formFields["client_id"] = _configuration.GetSection("Keycloak:ClientId").Value;
                formFields["client_secret"] = _configuration.GetSection("Keycloak:ClientSecret").Value;

                var formData = FormatFormData(formFields);
                context.Request.Body = new MemoryStream(formData);
                context.Request.ContentLength = formData.Length;
                context.Request.ContentType = "application/x-www-form-urlencoded";
            }
            else if (context.Request.Path.StartsWithSegments("/gateway/inventory/user/logout") && context.Request.Method == "POST")
            {
                var formContent = await context.Request.ReadFormAsync();
                var formFields = new Dictionary<string, string>();
                foreach (var field in formContent)
                {
                    formFields[field.Key] = field.Value;
                }

                formFields["client_id"] = _configuration.GetSection("Keycloak:ClientId").Value;
                formFields["client_secret"] = _configuration.GetSection("Keycloak:ClientSecret").Value;

                var formData = FormatFormData(formFields);
                context.Request.Body = new MemoryStream(formData);
                context.Request.ContentLength = formData.Length;
                context.Request.ContentType = "application/x-www-form-urlencoded";
            }


            await _next(context);
        }

        private static byte[] FormatFormData(Dictionary<string, string> formFields)
        {
            var formData = formFields
                .Select(field => $"{Uri.EscapeDataString(field.Key)}={Uri.EscapeDataString(field.Value)}")
                .ToList();

            return Encoding.UTF8.GetBytes(string.Join("&", formData));
        }
    }

    public static class FormContentMiddlewareExtensions
    {
        public static IApplicationBuilder UseFormContentMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<KeycloakFormContentMiddleware>();
        }
    }
}
