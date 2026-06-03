using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using ProjectName.Application.Common.Identity;
using ProjectName.Infrastructure.Common.Identity.Options;
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
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

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

            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddHttpContextAccessor();

            if (isDevelopment)
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(
                        Path.Combine(AppContext.BaseDirectory, ".data-protection-keys")));
            }

            services.AddEndpointsApiExplorer();
            services.AddProblemDetails();
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, _) =>
                {
                    var entraOptions = context.ApplicationServices.GetRequiredService<IOptions<EntraIDOptions>>().Value;
                    var baseUrl = entraOptions.Instance.TrimEnd('/');
                    var authUrl = new Uri($"{baseUrl}/{entraOptions.TenantId}/oauth2/v2.0/authorize");
                    var tokenUrl = new Uri($"{baseUrl}/{entraOptions.TenantId}/oauth2/v2.0/token");

                    var scopes = entraOptions.ApiScope is not null
                        ? new Dictionary<string, string> { { entraOptions.ApiScope, "Access API" } }
                        : new Dictionary<string, string>();

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = authUrl,
                                TokenUrl = tokenUrl,
                                Scopes = scopes
                            }
                        }
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["OAuth2"] = securityScheme;

                    var referenceScheme = new OpenApiSecuritySchemeReference("OAuth2", document);
                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement { [referenceScheme] = [] });

                    return Task.CompletedTask;
                });
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
