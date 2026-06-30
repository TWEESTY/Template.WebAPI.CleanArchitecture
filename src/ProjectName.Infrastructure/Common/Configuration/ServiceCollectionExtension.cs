using System.Text.RegularExpressions;
using System.Reflection;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Accounts.Common;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Persistence;
using ProjectName.Application.DogBreeds.Common;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Infrastructure.Common.Http;
using ProjectName.Infrastructure.Common.Identity.Options;
using ProjectName.Infrastructure.GraphApi;
using ProjectName.Infrastructure.GraphApi.Options;
using ProjectName.Infrastructure.HttpClients;
using ProjectName.Infrastructure.Persistence;
using ProjectName.Infrastructure.Persistence.Options;
using ProjectName.Infrastructure.Persistence.Repositories;
using Refit;

namespace ProjectName.Infrastructure.Common.Configuration;

/// <summary>
/// Provides extension methods for configuring infrastructure services in the application.
/// </summary>
public static partial class ServiceCollectionExtension
{
    private static readonly Regex _loginPathRegex = LoginPathRegex();

    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(ConfigurationManager configurationManager, bool isDevelopment)
        {
#pragma warning disable CA1062 // Validate arguments of public methods

            if (!IsBuildTimeOpenApiGeneration())
            {
                _ = services
                    .RegisterAuthentication(configurationManager)
                    .RegisterGraphApiOnBehalfApp(configurationManager)
                    .RegisterPersistence(configurationManager, isDevelopment);
            }

            _ = services
                .RegisterRepositories()
                .RegisterHttpClients();
#pragma warning restore CA1062 // Validate arguments of public methods

            _ = services.AddMemoryCache();

            return services;
        }

        private static bool IsBuildTimeOpenApiGeneration()
        {
            return string.Equals(
                Assembly.GetEntryAssembly()?.GetName().Name,
                "GetDocument.Insider",
                StringComparison.Ordinal)
                || string.Equals(
                    Environment.GetEnvironmentVariable("PROJECTNAME_OPENAPI_EXPORT"),
                    "true",
                    StringComparison.OrdinalIgnoreCase);
        }

        private IServiceCollection RegisterPersistence(ConfigurationManager configurationManager, bool isDevelopment)
        {
            _ = services.Configure<DataOptions>(configurationManager.GetSection(DataOptions.Key));

            _ = services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
            {
                DataOptions dataOptions = serviceProvider.GetRequiredService<IOptions<DataOptions>>().Value;

                string sqliteFileName = dataOptions.SqliteFileName;
                if (string.IsNullOrWhiteSpace(sqliteFileName))
                {
                    sqliteFileName = isDevelopment ? "projectname.dev.db" : "projectname.db";
                }

                string repositoryRoot = ResolveRepositoryRoot();
                string databaseFolder = Path.Combine(repositoryRoot, dataOptions.DatabaseRelativePathFromRepositoryRoot);
                _ = Directory.CreateDirectory(databaseFolder);

                string sqliteFilePath = Path.Combine(databaseFolder, sqliteFileName);
                _ = optionsBuilder.UseSqlite($"Data Source={sqliteFilePath}");
            });

            _ = services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

            _ = services.AddScoped<ApplicationDbContextInitialiser>();
            _ = services.AddScoped<IUnitOfWorkManager, EfUnitOfWorkManager>(
                serviceProvider => new EfUnitOfWorkManager(serviceProvider.GetRequiredService<AppDbContext>())
            );

            return services;
        }

        private static string ResolveRepositoryRoot()
        {
            string currentPath = Directory.GetCurrentDirectory();
            DirectoryInfo? directoryInfo = new(currentPath);

            while (directoryInfo is not null)
            {
                string packagesPropsPath = Path.Combine(directoryInfo.FullName, "Directory.Packages.props");
                if (File.Exists(packagesPropsPath))
                {
                    return directoryInfo.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            }

            return currentPath;
        }

        private IServiceCollection RegisterAuthentication(ConfigurationManager configurationManager)
        {
            _ = services.Configure<EntraIDOptions>(configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName));
            EntraIDOptions entraIDOptions = configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName).Get<EntraIDOptions>()!;

            DownstreamsApiOptions downstreamsApiOptions = configurationManager.GetSection(DownstreamsApiOptions.ConfigurationSectionName).Get<DownstreamsApiOptions>()!;
            _ = services.Configure<DownstreamsApiOptions>(configurationManager.GetSection(DownstreamsApiOptions.ConfigurationSectionName));
            _ = services.AddAuthentication()
                .AddMicrosoftIdentityWebApi(configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName), JwtBearerDefaults.AuthenticationScheme)
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddDistributedTokenCaches();

            _ = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddMicrosoftIdentityWebApp(configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName))
                .EnableTokenAcquisitionToCallDownstreamApi(downstreamsApiOptions.InitialScopes)
                    .AddMicrosoftGraph(configurationManager.GetSection(GraphApiOnBehalfUserOptions.ConfigurationSectionName))
                .AddDistributedTokenCaches();

            _ = services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
            });

            _ = services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events.OnRedirectToIdentityProvider += context =>
                {
                    Regex regex = _loginPathRegex;
                    if (regex.IsMatch(context.Request.Path))
                    {
                        return Task.CompletedTask;
                    }

                    // If the user is unauthenticated but it tries to access to another endpoints than login, just return 401
                    if (context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }

                    context.HandleResponse();

                    return Task.CompletedTask;
                };

                options.Events.OnTokenValidated += context =>
                {
                    // Do some checking here about the user and throw a specific exception if needed (for example if the user has no account in your application)
                    return Task.CompletedTask;
                };

                options.Events.OnRemoteFailure += context =>
                {
                    string errorMessage = "An error occurred during the authentication process.";

                    context.Response.Redirect($"{entraIDOptions.CustomErrorPath}?error={errorMessage}");

                    context.HandleResponse();

                    return Task.CompletedTask;
                };
            });

            return services;
        }

        private IServiceCollection RegisterGraphApiOnBehalfApp(ConfigurationManager configurationManager)
        {

            _ = services.Configure<GraphApiOnBehalfAppOptions>(configurationManager.GetSection(GraphApiOnBehalfAppOptions.ConfigurationNodeName));

            GraphApiOnBehalfAppOptions graphApiOptions = configurationManager.GetSection(GraphApiOnBehalfAppOptions.ConfigurationNodeName).Get<GraphApiOnBehalfAppOptions>()!;

            // Values from app registration
            string clientId = graphApiOptions.ClientId;
            string tenantId = graphApiOptions.TenantId;
            string clientSecret = graphApiOptions.ClientSecret;

            ClientSecretCredentialOptions options = new()
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret, options);

            _ = services.AddKeyedSingleton("GraphOnBehalfApp", (provider, _) =>
            {
                return new GraphServiceClient(tokenCredential: clientSecretCredential, graphApiOptions.DefaultScopes, baseUrl: graphApiOptions.BaseUrl);
            });

            return services;
        }

        private IServiceCollection RegisterRepositories()
        {
            _ = services.AddScoped<IClinicRepository, ClinicRepository>();
            _ = services.AddScoped<IPetRepository, PetRepository>();
            _ = services.AddScoped<IVeterinarianRepository, VeterinarianRepository>();
            _ = services.AddScoped<IVaccineRepository, VaccineRepository>();
            _ = services.AddScoped<IOwnerRepository, OwnerRepository>();
            _ = services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            return services;
        }

        private IServiceCollection RegisterHttpClients()
        {
            _ = services.AddTransient<RefitRequestLoggingHandler>();
            _ = services.AddScoped<IAccountPhotoService, PhotoService>();

            // Register Refit client for The Dog API
            _ = services.AddHttpClient<IDogBreedApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://dogapi.dog"))
                .AddHttpMessageHandler<RefitRequestLoggingHandler>()
                .AddTypedClient(RestService.For<IDogBreedApiClient>);

            // Register the breed details service
            _ = services.AddScoped<IDogBreedDetailsService, BreedDetailsService>();

            return services;
        }
    }

    [GeneratedRegex(@"^/api/v\d+/account/login$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "fr-FR")]
    private static partial Regex LoginPathRegex();
}
