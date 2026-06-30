using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Persistence;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Web.Api.Tests;

public sealed class CustomWebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _sqliteFolderRelativePath = Path.Combine("tests", ".testdb", "api", Guid.NewGuid().ToString("N"))
        .Replace('\\', '/');

    private readonly string _sqliteFileName = "api-tests.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("PROJECTNAME_OPENAPI_EXPORT", "true");

        _ = builder.UseEnvironment("Testing");

        _ = builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.Sources.Clear();

            _ = configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Data:DatabaseRelativePathFromRepositoryRoot"] = _sqliteFolderRelativePath,
                ["Data:SqliteFileName"] = _sqliteFileName,
                ["EntraID:Instance"] = "https://login.microsoftonline.com/",
                ["EntraID:ClientId"] = "48c716ab-d5e1-4997-a155-1322ed5ed9bd",
                ["EntraID:ClientSecret"] = "test-secret",
                ["EntraID:TenantId"] = "a4debdf3-9e4e-466a-85bf-73881152133d",
                ["EntraID:CallbackPath"] = "/signin-oidc",
                ["EntraID:SignedOutCallbackPath"] = "/signout-oidc",
                ["EntraID:ApiScope"] = "api://48c716ab-d5e1-4997-a155-1322ed5ed9bd/.default",
                ["EntraID:ErrorPath"] = "/error",
                ["EntraID:CustomErrorPath"] = "https://localhost/error",
                ["DownstreamsApi:InitialScopes:0"] = "User.Read",
                ["DownstreamsApi:GraphApi:BaseUrl"] = "https://graph.microsoft.com/v1.0",
                ["DownstreamsApi:GraphApi:Scopes:0"] = "User.Read",
                ["GraphApiOnBehalfApp:DefaultScopes:0"] = "https://graph.microsoft.com/.default",
                ["GraphApiOnBehalfApp:TenantId"] = "a4debdf3-9e4e-466a-85bf-73881152133d",
                ["GraphApiOnBehalfApp:ClientId"] = "48c716ab-d5e1-4997-a155-1322ed5ed9bd",
                ["GraphApiOnBehalfApp:ClientSecret"] = "test-secret",
                ["GraphApiOnBehalfApp:BaseUrl"] = "https://graph.microsoft.com/v1.0"
            });
        });

        _ = builder.ConfigureTestServices(services =>
        {
            string sqliteFolderAbsolutePath = Path.Combine(GetRepositoryRoot(), _sqliteFolderRelativePath);
            _ = Directory.CreateDirectory(sqliteFolderAbsolutePath);
            string sqliteFilePath = Path.Combine(sqliteFolderAbsolutePath, _sqliteFileName);

            _ = services.AddDbContext<AppDbContext>(options =>
            {
                _ = options.UseSqlite($"Data Source={sqliteFilePath}");
            });

            _ = services.AddScoped<ApplicationDbContextInitialiser>();
            _ = services.AddScoped<IUnitOfWorkManager, TestUnitOfWorkManager>();

            _ = services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            _ = services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(TestAuthHandler.SchemeName)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        });
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        Environment.SetEnvironmentVariable("PROJECTNAME_OPENAPI_EXPORT", null);

        string sqliteFolderAbsolutePath = Path.Combine(GetRepositoryRoot(), _sqliteFolderRelativePath);
        if (Directory.Exists(sqliteFolderAbsolutePath))
        {
            try
            {
                Directory.Delete(sqliteFolderAbsolutePath, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup only.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort cleanup only.
            }
        }

        TryDeleteEmptyDirectory(Path.Combine(GetRepositoryRoot(), "tests", ".testdb"));
    }

    private static string GetRepositoryRoot()
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

    private static void TryDeleteEmptyDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        try
        {
            Directory.Delete(directoryPath, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup only.
        }
        catch (UnauthorizedAccessException)
        {
            // Best-effort cleanup only.
        }
    }
}
