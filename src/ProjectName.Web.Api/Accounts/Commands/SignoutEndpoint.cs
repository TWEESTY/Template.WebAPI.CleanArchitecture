using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectName.Infrastructure.Common.Identity.Options;

namespace ProjectName.Web.Api.Accounts.Commands;

public static class SignoutEndpoint
{
    public static async Task<Results<RedirectHttpResult, InternalServerError>> HandleAsync(
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IOptions<EntraIDOptions> entraIDOptions,
        CancellationToken cancellationToken)
    {
        await httpContextAccessor.HttpContext!.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await httpContextAccessor.HttpContext!.SignOutAsync();
        var signoutUrl = entraIDOptions.Value.SignedOutCallbackPath;
        return TypedResults.Redirect(signoutUrl);
    }
}
