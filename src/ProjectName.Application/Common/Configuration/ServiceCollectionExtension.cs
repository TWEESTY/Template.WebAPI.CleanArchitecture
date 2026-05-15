using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.PipelineBehaviors;

namespace ProjectName.Application.Common.Configuration;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices(bool isDevelopment)
        {
            services.AddMediator(options =>
            {
                options.Assemblies = [Assembly.GetAssembly(typeof(ServiceCollectionExtension))];
                options.PipelineBehaviors = [
                    typeof(LoggingBehavior<,>),
                    typeof(UnhandledExceptionBehavior<,>)
                ];
            });

            return services;
        }
    }
}
