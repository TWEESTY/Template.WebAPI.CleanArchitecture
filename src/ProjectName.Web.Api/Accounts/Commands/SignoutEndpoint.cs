using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectName.Infrastructure.Common.Identity.Options;

namespace ProjectName.Web.Api.Accounts.Commands;

/// <summary>
/// Represents the endpoint for signing out a user from the application. This endpoint handles the sign-out process by signing the user out of the OpenID Connect authentication scheme and redirecting them to a specified callback path after sign-out.
/// </summary>
internal static class SignoutEndpoint
{
    internal static async Task<Results<RedirectHttpResult, InternalServerError>> HandleAsync(
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IOptions<EntraIDOptions> entraIDOptions)
    {
        await httpContextAccessor.HttpContext!.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await httpContextAccessor.HttpContext!.SignOutAsync();
        string signoutUrl = entraIDOptions.Value.SignedOutCallbackPath;
        return TypedResults.Redirect(signoutUrl);
    }
}
