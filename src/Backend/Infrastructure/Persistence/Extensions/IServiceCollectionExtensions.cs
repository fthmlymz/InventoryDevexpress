using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Persistence.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContexts(services, configuration);
            services.AddRepositories();
        }

        private static void AddDbContexts(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServerConnection");
            AddDbContext<ApplicationDbContext>(services, connectionString, typeof(ApplicationDbContext));

            var capConnectionString = configuration.GetConnectionString("CapLogSqlServerConnection");
            AddDbContext<DotnetCapDbContext>(services, capConnectionString, typeof(DotnetCapDbContext));

            var logger = CreateLogger(configuration);
            services.AddLogging(x => x.AddSerilog(logger));
        }

        private static void AddDbContext<TDbContext>(IServiceCollection services, string connectionString, Type assemblyType) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(options =>
                options.UseSqlServer(connectionString,
                    builder => builder.MigrationsAssembly(assemblyType.Assembly.GetName().Name).MigrationsHistoryTable("__EFMigrationsHistory")));
        }

        private static ILogger CreateLogger(IConfiguration configuration)
        {
            var serilogSeqUrl = configuration.GetSection("SerilogSeqUrl").Value;
            var serilogConnectionString = configuration.GetConnectionString("SeriLogConnection");
            var minimumLevel = configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default");

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.Seq(serilogSeqUrl)
                .WriteTo.MSSqlServer(
                    connectionString: serilogConnectionString,
                    sinkOptions: new MSSqlServerSinkOptions { AutoCreateSqlDatabase = true, AutoCreateSqlTable = true, TableName = "LogEvents" });

            return loggerConfiguration.CreateLogger();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddScoped<ICompanyRepository, CompanyRepository>();
        }
    }
}
