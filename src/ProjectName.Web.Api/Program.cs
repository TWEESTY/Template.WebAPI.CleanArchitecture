using ProjectName.Application.Common.Configuration;
using ProjectName.Infrastructure.Common.Configuration;
using ProjectName.Infrastructure.Persistence;
using ProjectName.Web.Api.Common.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up.");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.AddServiceDefaults();

    bool isDevelopment = builder.Environment.IsDevelopment();

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
      loggerConfiguration.ReadFrom.Configuration(context.Configuration)
    );

    builder.Services
        .AddWebApiServices(isDevelopment)
        .AddApplicationServices(isDevelopment)
        .AddInfrastructureServices(builder.Configuration, isDevelopment);



    var app = builder.Build();
    await app.ConfigureWebApplicationAsync(isDevelopment);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.Information("Shutdown completed.");
    Log.CloseAndFlush();
}