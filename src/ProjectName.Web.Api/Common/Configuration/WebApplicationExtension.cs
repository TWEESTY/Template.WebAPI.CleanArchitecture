using Scalar.AspNetCore;
using Serilog;

namespace ProjectName.Web.Api.Common.Configuration;

public static class WebApplicationExtension
{
    extension(WebApplication app)
    {
        public WebApplication ConfigureWebApplication(bool isDevelopment)
        {
            app.UseRequestLocalization();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages();

            // Configure the HTTP request pipeline.
            if (isDevelopment)
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();
            app.UseHttpLogging();

            app.AddEndpointsGroups();

            return app;
        }

        private void AddEndpointsGroups()
        {
            // app.MapHealthChecks("/health").AllowAnonymous();
        }
    }
}
