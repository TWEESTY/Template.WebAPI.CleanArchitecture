using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using ProjectName.Application.Common.Identity;
using ProjectName.Infrastructure.Common.Identity.Options;
using ProjectName.Web.Api.Common.Identity;
using ProjectName.Web.Api.Common.Problems;
namespace ProjectName.Web.Api.Common.Configuration;

/// <summary>
/// Extension methods for configuring web API services, including authentication, authorization, localization, HTTP logging, OpenAPI documentation, and exception handling.
/// </summary>
public static class ServiceCollectionExtension
{
    private static readonly string[] _supportedCultures = ["en", "fr"];

    extension(IServiceCollection services)
    {
        public IServiceCollection AddWebApiServices(bool isDevelopment)
        {
            _ = services.AddAuthorization(options =>
            {
                AuthorizationPolicyBuilder defaultAuthorizationPolicyBuilder = new(
                    JwtBearerDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            _ = services.AddLocalization();
            _ = services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(_supportedCultures[0])
                    .AddSupportedCultures(_supportedCultures)
                    .AddSupportedUICultures(_supportedCultures)
                    .ApplyCurrentCultureToResponseHeaders = true;
            });

            _ = services.AddHttpLogging(o =>
            {
                o.CombineLogs = true;

                o.LoggingFields = HttpLoggingFields.RequestQuery
                | HttpLoggingFields.RequestMethod
                | HttpLoggingFields.RequestPath
                | HttpLoggingFields.RequestBody
                | HttpLoggingFields.ResponseStatusCode
                | HttpLoggingFields.Duration;
            });

            _ = services.AddScoped<ICurrentUser, CurrentUser>();

            _ = services.AddHttpContextAccessor();

            if (isDevelopment)
            {
                _ = services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(
                        Path.Combine(AppContext.BaseDirectory, ".data-protection-keys")));
            }

            _ = services.AddEndpointsApiExplorer();
            _ = services.AddProblemDetails();
            _ = services.AddOpenApi(options =>
            {
                _ = options.AddDocumentTransformer((document, context, _) =>
                {
                    EntraIDOptions entraOptions = context.ApplicationServices.GetRequiredService<IOptions<EntraIDOptions>>().Value;
                    string baseUrl = entraOptions.Instance.TrimEnd('/');
                    Uri authUrl = new($"{baseUrl}/{entraOptions.TenantId}/oauth2/v2.0/authorize");
                    Uri tokenUrl = new($"{baseUrl}/{entraOptions.TenantId}/oauth2/v2.0/token");

                    Dictionary<string, string> scopes = entraOptions.ApiScope is not null
                        ? new Dictionary<string, string> { { entraOptions.ApiScope, "Access API" } }
                        : [];

                    OpenApiSecurityScheme securityScheme = new()
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

                    OpenApiSecuritySchemeReference referenceScheme = new("OAuth2", document);
                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement { [referenceScheme] = [] });

                    return Task.CompletedTask;
                });
            });
            _ = services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
