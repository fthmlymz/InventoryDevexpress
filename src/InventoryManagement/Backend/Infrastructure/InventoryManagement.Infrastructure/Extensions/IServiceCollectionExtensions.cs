using EasyCaching.Core.Configurations;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Common.Interfaces;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Infrastructure.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddTransient<IDateTimeService, DateTimeService>();

            #region Redis
            var cacheSettings = configuration.GetSection("RedisConnectionSettings").Get<CacheSettings>();
            services.AddEasyCaching(option =>
            {
                option.WithJson();
                option.UseRedis(config =>
                {
                    config.DBConfig.ConnectionTimeout = 5000;
                    config.DBConfig.Database = cacheSettings.Database;
                    config.DBConfig.Endpoints.Add(new ServerEndPoint(cacheSettings.RedisURL, cacheSettings.RedisPort));
                }, "json");
            });
            services.AddTransient<IEasyCacheService, EasyCacheService>();
            #endregion
        }        
    }
}



/* Original code
         * public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);
        }

        private static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<IMediator, Mediator>()
                .AddTransient<IDomainEventDispatcher, DomainEventDispatcher>()
                .AddTransient<IDateTimeService, DateTimeService>();
            //.AddTransient<IEmailService, EmailService>();


            #region Redis
            services.Configure<CacheSettings>(configuration.GetSection("RedisConnectionSettings"));
            var cacheSettings = configuration.GetSection("RedisConnectionSettings").Get<CacheSettings>();
            services.AddEasyCaching(option =>
            {
                option.WithJson();
                option.UseRedis(config =>
                {
                    config.DBConfig.ConnectionTimeout = 5000;
                    config.DBConfig.Database = cacheSettings.Database;
                    config.DBConfig.Endpoints.Add(new ServerEndPoint(cacheSettings.RedisURL, cacheSettings.RedisPort));
                }, "json");
            });
            services.AddTransient<IEasyCacheService, EasyCacheService>();
            #endregion
        }*/