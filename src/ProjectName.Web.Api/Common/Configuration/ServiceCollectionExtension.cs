using Microsoft.AspNetCore.HttpLogging;
using ProjectName.Application.Common.Identity;
using ProjectName.Web.Api.Common.Identity;
using ProjectName.Web.Api.Common.Problems;
namespace ProjectName.Web.Api.Common.Configuration;

public static class ServiceCollectionExtension
{
    private static readonly string[] _supportedCultures = ["en", "fr"];

    extension(IServiceCollection services)
    {
        public IServiceCollection AddWebApiServices(bool isDevelopment)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(_supportedCultures[0])
                    .AddSupportedCultures(_supportedCultures)
                    .AddSupportedUICultures(_supportedCultures)
                    .ApplyCurrentCultureToResponseHeaders = true;
            });

            services.AddHttpLogging(o =>
            {
                o.CombineLogs = true;

                o.LoggingFields = HttpLoggingFields.RequestQuery
                    | HttpLoggingFields.RequestMethod
                    | HttpLoggingFields.RequestPath
                    | HttpLoggingFields.RequestBody
                    | HttpLoggingFields.ResponseStatusCode
                    | HttpLoggingFields.ResponseBody
                    | HttpLoggingFields.Duration;
            });

            services.AddScoped<IUser, CurrentUser>();

            services.AddHttpContextAccessor();

            services.AddEndpointsApiExplorer();
            services.AddProblemDetails();
            services.AddOpenApi();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
