using Microsoft.Extensions.Options;
using ProjectName.Infrastructure.Common.Identity.Options;
using ProjectName.Web.Api.Accounts;
using ProjectName.Web.Api.Pets;
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
                var entraOptions = app.Services.GetRequiredService<IOptions<EntraIDOptions>>().Value;
                app.MapScalarApiReference(options => options
                    .AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2", flow =>
                    {
                        flow.ClientId = entraOptions.ClientId;
                        flow.ClientSecret = entraOptions.ClientSecret;
                        flow.Pkce = Pkce.No;
                        flow.RedirectUri = "https://localhost:7310/scalar/v1";
                        flow.CredentialsLocation = CredentialsLocation.Body;
                    })
                    .EnablePersistentAuthentication());
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();
            app.UseHttpLogging();

            app.AddEndpointsGroups();

            return app;
        }

        private void AddEndpointsGroups()
        {
            AccountsEndpointsGroup.MapAccountsEndpoints(app);
            PetsEndpointsGroup.MapPetsEndpoints(app);
        }
    }
}
