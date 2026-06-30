using ProjectName.Web.Api.Veterinarians.Commands;
using ProjectName.Web.Api.Veterinarians.Queries;

namespace ProjectName.Web.Api.Veterinarians;

/// <summary>
/// Represents a group of endpoints related to veterinarian management, including creating, retrieving, updating, and deleting veterinarians. This class provides methods to map the endpoints to the application's routing system and configure their behavior, such as authorization requirements and display information.
/// </summary>
internal static class VeterinariansEndpointsGroup
{
    public const string BasePath = "/api/veterinarians";
    public const string GroupName = "Veterinarians";

    public static RouteGroupBuilder MapVeterinariansEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet(string.Empty, GetVeterinariansEndpoint.HandleAsync);
        _ = group.MapGet("{id:guid}", GetVeterinarianByIdEndpoint.HandleAsync);
        _ = group.MapPost(string.Empty, CreateVeterinarianEndpoint.HandleAsync);
        _ = group.MapPut("{id:guid}", UpdateVeterinarianEndpoint.HandleAsync);
        _ = group.MapDelete("{id:guid}", DeleteVeterinarianEndpoint.HandleAsync);

        return group;
    }
}
