using Microsoft.AspNetCore.Mvc;
using ProjectName.Web.Api.Accounts.Commands;
using ProjectName.Web.Api.Accounts.Queries;

namespace ProjectName.Web.Api.Accounts;

public static class AccountsEndpointsGroup
{
    public const string BasePath = "/api/accounts";
    public const string GroupName = "Accounts";

    public static RouteGroupBuilder MapAccountsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet("login", ([FromQuery] string redirectUrl) => Results.Redirect(redirectUrl))
            .AllowAnonymous()
            .WithDisplayName("Login")
            .WithSummary("Redirects to the login page.")
            .WithDescription("This endpoint redirects the user to the login page.");

        group.MapGet("signout", SignoutEndpoint.HandleAsync)
            .AllowAnonymous()
            .WithDisplayName("Signout")
            .WithSummary("Signs out the user.")
            .WithDescription("This endpoint signs out the user and redirects to the signed-out callback URL.");

        group.MapGet("claims", GetClaimsEndpoint.HandleAsync)
            .WithDisplayName("Get User Claims")
            .WithSummary("Retrieves the claims of the authenticated user.")
            .WithDescription("This endpoint returns the claims associated with the currently authenticated user.");

        return group;
    }
}
