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
    //Best performance
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


/* Original code
     public static class IServiceCollectionExtensions
    {

        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddMappings();
            services.AddDbContext(configuration);
            services.AddRepositories();
        }

        //private static void AddMappings(this IServiceCollection services)
        //{
        //    services.AddAutoMapper(Assembly.GetExecutingAssembly());
        //}


       

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            #region Application Database connection
            var connectionString = configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString,
                   builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name).MigrationsHistoryTable("__EFMigrationsHistory")));
            #endregion


            #region Event Drive - DotnetCap log
            var capConnectionString = configuration.GetConnectionString("CapLogSqlServerConnection");
            services.AddDbContext<DotnetCapDbContext>(options =>
               options.UseSqlServer(capConnectionString,
                   builder => builder.MigrationsAssembly(typeof(DotnetCapDbContext).Assembly.GetName().Name).MigrationsHistoryTable("__EFMigrationsHistory")));
            #endregion


            #region Serilog Logging
            var logger = new LoggerConfiguration()
              //.MinimumLevel.Information()
              .MinimumLevel.Warning()
              .WriteTo.Seq(Convert.ToString(configuration.GetSection("SerilogSeqUrl").Value))
              .WriteTo.MSSqlServer(
                connectionString: configuration.GetConnectionString("SeriLogConnection"),
                sinkOptions: new MSSqlServerSinkOptions { AutoCreateSqlDatabase = true, AutoCreateSqlTable = true, TableName = "LogEvents" })
              .CreateLogger();
            services.AddLogging(x => x.AddSerilog(logger));
            #endregion
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
                .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddTransient<ICompanyRepository, CompanyRepository>();
        }
    }*/