using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectName.Application.Common.Configuration;
using ProjectName.Application.DogBreeds.Common;
using ProjectName.Infrastructure.Common.Configuration;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests;

public sealed class SqliteApplicationTestFixture : IAsyncLifetime
{
    private readonly string _sqliteFolderRelativePath = Path.Combine("tests", ".testdb", "application", Guid.NewGuid().ToString("N"))
        .Replace('\\', '/');

    private readonly string _sqliteFileName = "application-tests.db";

    private ServiceProvider? _serviceProvider;

    public IServiceProvider Services => _serviceProvider ?? throw new InvalidOperationException("The fixture has not been initialized.");

    public async Task InitializeAsync()
    {
        ConfigurationManager configuration = BuildTestConfiguration();

        ServiceCollection services = [];
        _ = services.AddLogging();
        _ = services
            .AddApplicationServices()
            .AddInfrastructureServices(configuration, isDevelopment: false);

        _ = services.RemoveAll<IDogBreedDetailsService>();
        _ = services.AddScoped<IDogBreedDetailsService, TestDogBreedDetailsService>();

        _serviceProvider = services.BuildServiceProvider();

        using IServiceScope scope = _serviceProvider.CreateScope();
        ApplicationDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            _serviceProvider?.Dispose();
        }

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

    private ConfigurationManager BuildTestConfiguration()
    {
        ConfigurationManager configuration = new();
        _ = configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Data:DatabaseRelativePathFromRepositoryRoot"] = _sqliteFolderRelativePath,
            ["Data:SqliteFileName"] = _sqliteFileName,
            ["EntraID:Instance"] = "https://login.microsoftonline.com/",
            ["EntraID:ClientId"] = "00000000-0000-0000-0000-000000000001",
            ["EntraID:ClientSecret"] = "test-secret",
            ["EntraID:TenantId"] = "00000000-0000-0000-0000-000000000002",
            ["EntraID:CallbackPath"] = "/signin-oidc",
            ["EntraID:SignedOutCallbackPath"] = "/signout-oidc",
            ["EntraID:ApiScope"] = "api://00000000-0000-0000-0000-000000000001/.default",
            ["EntraID:ErrorPath"] = "/error",
            ["EntraID:CustomErrorPath"] = "https://localhost/error",
            ["DownstreamsApi:InitialScopes:0"] = "User.Read",
            ["DownstreamsApi:GraphApi:BaseUrl"] = "https://graph.microsoft.com/v1.0",
            ["DownstreamsApi:GraphApi:Scopes:0"] = "User.Read",
            ["GraphApiOnBehalfApp:DefaultScopes:0"] = "https://graph.microsoft.com/.default",
            ["GraphApiOnBehalfApp:TenantId"] = "00000000-0000-0000-0000-000000000002",
            ["GraphApiOnBehalfApp:ClientId"] = "00000000-0000-0000-0000-000000000001",
            ["GraphApiOnBehalfApp:ClientSecret"] = "test-secret",
            ["GraphApiOnBehalfApp:BaseUrl"] = "https://graph.microsoft.com/v1.0"
        });

        return configuration;
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
