using Microsoft.AspNetCore.Mvc;
using ProjectName.Web.Api.Accounts.Commands;
using ProjectName.Web.Api.Accounts.Queries;

namespace ProjectName.Web.Api.Accounts;

/// <summary>
/// Represents a group of endpoints related to account management, including login, signout, and retrieving user claims. This class provides methods to map the endpoints to the application's routing system and configure their behavior, such as authorization requirements and display information.
/// </summary>
public static class AccountsEndpointsGroup
{
    public const string BasePath = "/api/accounts";
    public const string GroupName = "Accounts";

    public static RouteGroupBuilder MapAccountsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet("login", ([FromQuery] string redirectUrl) => Results.Redirect(redirectUrl))
            .AllowAnonymous()
            .WithDisplayName("Login")
            .WithSummary("Redirects to the login page.")
            .WithDescription("This endpoint redirects the user to the login page.");

        _ = group.MapGet("signout", SignoutEndpoint.HandleAsync)
            .AllowAnonymous()
            .WithDisplayName("Signout")
            .WithSummary("Signs out the user.")
            .WithDescription("This endpoint signs out the user and redirects to the signed-out callback URL.");

        _ = group.MapGet("claims", GetClaimsEndpoint.HandleAsync)
            .WithDisplayName("Get User Claims")
            .WithSummary("Retrieves the claims of the authenticated user.")
            .WithDescription("This endpoint returns the claims associated with the currently authenticated user.");

        _ = group.MapGet("photo", GetPhotoEndpoint.HandleAsync)
            .WithDisplayName("Get Photo")
            .WithSummary("Retrieves the authenticated user's profile photo from Graph API.")
            .WithDescription("This endpoint returns the photo bytes of the currently authenticated user from Microsoft Graph.");

        return group;
    }
}
