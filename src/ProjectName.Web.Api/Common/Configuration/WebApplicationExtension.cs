using Microsoft.Extensions.Options;
using System.Reflection;
using ProjectName.Infrastructure.Common.Identity.Options;
using ProjectName.Infrastructure.Persistence;
using ProjectName.Web.Api.Accounts;
using ProjectName.Web.Api.Appointments;
using ProjectName.Web.Api.Clinics;
using ProjectName.Web.Api.DogBreeds;
using ProjectName.Web.Api.Owners;
using ProjectName.Web.Api.Pets;
using ProjectName.Web.Api.Vaccines;
using ProjectName.Web.Api.Veterinarians;
using Scalar.AspNetCore;
using ProjectName.Web.Api.Common.Logs;

namespace ProjectName.Web.Api.Common.Configuration;

/// <summary>
/// Extension methods for configuring the web application, including middleware, authentication, authorization, localization, HTTP logging, OpenAPI documentation, and endpoint mapping.
/// </summary>
public static class WebApplicationExtension
{
    extension(WebApplication app)
    {
        public async Task<WebApplication> ConfigureWebApplicationAsync(bool isDevelopment)
        {
            bool isOpenApiExportExecution = IsOpenApiExportExecution();

            _ = app.UseRequestLocalization();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseStatusCodePages();

            if (isDevelopment || isOpenApiExportExecution)
            {
                _ = app.MapOpenApi();
            }

            if (isDevelopment && !isOpenApiExportExecution)
            {
                EntraIDOptions entraOptions = app.Services.GetRequiredService<IOptions<EntraIDOptions>>().Value;
                _ = app.MapScalarApiReference(options => options
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

                using IServiceScope scope = app.Services.CreateScope();
                ApplicationDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                await initialiser.InitialiseAsync();
                await initialiser.SeedAsync();

                _ = app.MapHealthChecks("/health");
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseHttpLogging();
            _ = app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.AddEndpointsGroups();

            return app;
        }

        private static bool IsOpenApiExportExecution()
        {
            return string.Equals(
                       Assembly.GetEntryAssembly()?.GetName().Name,
                       "GetDocument.Insider",
                       StringComparison.OrdinalIgnoreCase);
        }

        private void AddEndpointsGroups()
        {
            _ = AccountsEndpointsGroup.MapAccountsEndpoints(app);
            _ = PetsEndpointsGroup.MapPetsEndpoints(app);
            _ = ClinicsEndpointsGroup.MapClinicsEndpoints(app);
            _ = VeterinariansEndpointsGroup.MapVeterinariansEndpoints(app);
            _ = VaccinesEndpointsGroup.MapVaccinesEndpoints(app);
            _ = OwnersEndpointsGroup.MapOwnersEndpoints(app);
            _ = AppointmentsEndpointsGroup.MapAppointmentsEndpoints(app);
            _ = DogBreedsEndpointsGroup.MapDogBreedsEndpoints(app);
        }
    }
}
