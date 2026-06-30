using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.PipelineBehaviors;

namespace ProjectName.Application.Common.Configuration;

/// <summary>
/// Extension methods for configuring application services.
/// </summary>
public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            _ = services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(ServiceCollectionExtension))!);

            return services.AddMediator(options =>
            {
                options.Assemblies = [Assembly.GetAssembly(typeof(ServiceCollectionExtension))];
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.PipelineBehaviors = [
                    typeof(LoggingBehavior<,>),
                    typeof(ValidationBehavior<,>),
                    typeof(UnitOfWorkBehavior<,>),
                    typeof(UnhandledExceptionBehavior<,>)
                ];
            });
        }
    }
}
