using Application.Behaviors;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryManagement.Application.Common.Exceptions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMapster();
            services.AddMediator();
            services.AddValidators();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;

                opt.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);
                    throw new BadRequestExceptionCustom(problemDetails.Errors);
                };
            });
        }

        private static void AddMapster(this IServiceCollection services)
        {
            var assemblies = new[] { Assembly.GetExecutingAssembly() };
            TypeAdapterConfig.GlobalSettings.Scan(assemblies);
        }

        private static void AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        private static void AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}
