using System.Text.RegularExpressions;
using System.IO;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Infrastructure.Common.Identity.Options;
using ProjectName.Infrastructure.GraphApi.Options;
using ProjectName.Infrastructure.Persistence;
using ProjectName.Infrastructure.Persistence.Options;
using ProjectName.Infrastructure.Persistence.Repositories;

namespace ProjectName.Infrastructure.Common.Configuration;

public static class ServiceCollectionExtension
{
    private static readonly Regex LoginPathRegex = new(@"^/api/v\d+/account/login$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    extension(IServiceCollection services)
    {        
        public IServiceCollection AddInfrastructureServices(ConfigurationManager configurationManager, bool isDevelopment)
        {
            RegisterAuthentication(services, configurationManager)
                .RegisterPersistence(configurationManager, isDevelopment)
                .RegisterGraphApiOnBehalfApp(configurationManager)
                .RegisterRepositories(configurationManager)
                .RegisterHttpClients(configurationManager);

            services.AddMemoryCache();

            return services;
        }

        private IServiceCollection RegisterPersistence(ConfigurationManager configurationManager, bool isDevelopment)
        {
            services.Configure<DataOptions>(configurationManager.GetSection(DataOptions.Key));

            services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
            {
                DataOptions dataOptions = serviceProvider.GetRequiredService<IOptions<DataOptions>>().Value;

                string sqliteFileName = dataOptions.SqliteFileName;
                if (string.IsNullOrWhiteSpace(sqliteFileName))
                {
                    sqliteFileName = isDevelopment ? "projectname.dev.db" : "projectname.db";
                }

                string repositoryRoot = ResolveRepositoryRoot();
                string databaseFolder = Path.Combine(repositoryRoot, dataOptions.DatabaseRelativePathFromRepositoryRoot);
                Directory.CreateDirectory(databaseFolder);

                string sqliteFilePath = Path.Combine(databaseFolder, sqliteFileName);
                optionsBuilder.UseSqlite($"Data Source={sqliteFilePath}");
            });

            services.AddScoped<ApplicationDbContextInitialiser>();

            return services;
        }

        private static string ResolveRepositoryRoot()
        {
            string currentPath = Directory.GetCurrentDirectory();
            var directoryInfo = new DirectoryInfo(currentPath);

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
            services.Configure<EntraIDOptions>(configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName));
            EntraIDOptions entraIDOptions = configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName).Get<EntraIDOptions>()!;

            DownstreamsApiOptions downstreamsApiOptions = configurationManager.GetSection(DownstreamsApiOptions.ConfigurationSectionName).Get<DownstreamsApiOptions>()!;
            services.Configure<DownstreamsApiOptions>(configurationManager.GetSection(DownstreamsApiOptions.ConfigurationSectionName));
            services.AddAuthentication()
                .AddMicrosoftIdentityWebApi(configurationManager.GetSection(EntraIDOptions.ConfigurationSectionName), JwtBearerDefaults.AuthenticationScheme)
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddDistributedTokenCaches();

            services.AddAuthentication(options => {
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

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events.OnRedirectToIdentityProvider += context =>
                {
                    var regex = LoginPathRegex;
                    if (regex.IsMatch(context.Request.Path))
                        return Task.CompletedTask;

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

            services.Configure<GraphApiOnBehalfAppOptions>(configurationManager.GetSection(GraphApiOnBehalfAppOptions.ConfigurationNodeName));

            GraphApiOnBehalfAppOptions graphApiOptions = configurationManager.GetSection(GraphApiOnBehalfAppOptions.ConfigurationNodeName).Get<GraphApiOnBehalfAppOptions>()!;

            // Values from app registration
            string clientId = graphApiOptions.ClientId;
            string tenantId = graphApiOptions.TenantId;
            string clientSecret = graphApiOptions.ClientSecret;

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            services.AddKeyedSingleton<GraphServiceClient>("GraphOnBehalfApp", (provider, _) =>
            {
                return new GraphServiceClient(tokenCredential: clientSecretCredential, graphApiOptions.DefaultScopes, baseUrl: graphApiOptions.BaseUrl);
            });

            return services;
        }
   
        private IServiceCollection RegisterRepositories(ConfigurationManager configurationManager)
        {
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IVeterinarianRepository, VeterinarianRepository>();
            services.AddScoped<IVaccineRepository, VaccineRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            return services;
        }

        private IServiceCollection RegisterHttpClients(ConfigurationManager configurationManager)
        {
            // Register your HttpClients here
            return services;
        }
    }
}
