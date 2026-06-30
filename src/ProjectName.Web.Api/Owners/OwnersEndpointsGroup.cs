using ProjectName.Web.Api.Owners.Commands;
using ProjectName.Web.Api.Owners.Queries;

namespace ProjectName.Web.Api.Owners;

/// <summary>
/// Represents a group of endpoints related to owner management, including creating, retrieving, updating, and deleting owners. This class provides methods to map the endpoints to the application's routing system and configure their behavior, such as authorization requirements and display information.
/// </summary>
public static class OwnersEndpointsGroup
{
    public const string BasePath = "/api/owners";
    public const string GroupName = "Owners";

    public static RouteGroupBuilder MapOwnersEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet(string.Empty, GetOwnersEndpoint.HandleAsync);
        _ = group.MapGet("{id:guid}", GetOwnerByIdEndpoint.HandleAsync);
        _ = group.MapPost(string.Empty, CreateOwnerEndpoint.HandleAsync);
        _ = group.MapPost("with-initial-pet", CreateOwnerWithInitialPetEndpoint.HandleAsync);
        _ = group.MapPut("{id:guid}", UpdateOwnerEndpoint.HandleAsync);
        _ = group.MapDelete("{id:guid}", DeleteOwnerEndpoint.HandleAsync);

        return group;
    }
}
