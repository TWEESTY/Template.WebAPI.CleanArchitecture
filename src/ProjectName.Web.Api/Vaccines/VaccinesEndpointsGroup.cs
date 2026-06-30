using ProjectName.Web.Api.Vaccines.Commands;
using ProjectName.Web.Api.Vaccines.Queries;

namespace ProjectName.Web.Api.Vaccines;

/// <summary>
/// Represents a group of endpoints related to vaccine management, including creating, retrieving, updating, and deleting vaccines. This class provides methods to map the endpoints to the application's routing system and configure their behavior, such as authorization requirements and display information.
/// </summary>
internal static class VaccinesEndpointsGroup
{
    public const string BasePath = "/api/vaccines";
    public const string GroupName = "Vaccines";

    public static RouteGroupBuilder MapVaccinesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet(string.Empty, GetVaccinesEndpoint.HandleAsync);
        _ = group.MapGet("{id:guid}", GetVaccineByIdEndpoint.HandleAsync);
        _ = group.MapPost(string.Empty, CreateVaccineEndpoint.HandleAsync);
        _ = group.MapPut("{id:guid}", UpdateVaccineEndpoint.HandleAsync);
        _ = group.MapDelete("{id:guid}", DeleteVaccineEndpoint.HandleAsync);

        return group;
    }
}
