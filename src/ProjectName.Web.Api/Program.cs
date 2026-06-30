using System.Reflection;
using ProjectName.Application.Common.Configuration;
using ProjectName.Infrastructure.Common.Configuration;
using ProjectName.Web.Api.Common.Configuration;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    bool isBuildTimeOpenApiGeneration = string.Equals(
        Assembly.GetEntryAssembly()?.GetName().Name,
        "GetDocument.Insider",
        StringComparison.Ordinal);

    if (!isBuildTimeOpenApiGeneration)
    {
        _ = builder.AddServiceDefaults();
    }

    bool isDevelopment = builder.Environment.IsDevelopment();
    _ = builder.Services
        .AddWebApiServices(isDevelopment)
        .AddApplicationServices()
        .AddInfrastructureServices(builder.Configuration, isDevelopment);

    WebApplication app = builder.Build();
    _ = await app.ConfigureWebApplicationAsync(isDevelopment);

    if (!isBuildTimeOpenApiGeneration)
    {
        app.Run();
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Host terminated unexpectedly: {ex}");
    throw;
}
