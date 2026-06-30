using ProjectName.Web.Api.Clinics.Commands;
using ProjectName.Web.Api.Clinics.Queries;

namespace ProjectName.Web.Api.Clinics;

/// <summary>
/// Represents a group of endpoints related to clinic management, including creating, retrieving, updating, and deleting clinics, as well as managing veterinarians in clinics. This class provides methods to map the endpoints to the application's routing system and configure their behavior, such as authorization requirements and display information.
/// </summary>
public static class ClinicsEndpointsGroup
{
    public const string BasePath = "/api/clinics";
    public const string GroupName = "Clinics";

    public static RouteGroupBuilder MapClinicsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet(string.Empty, GetClinicsEndpoint.HandleAsync)
            .WithDisplayName("Get Clinics")
            .WithSummary("Retrieves a list of clinics.")
            .WithDescription("This endpoint returns a list of all clinics in the system.");

        _ = group.MapGet("{id:guid}", GetClinicByIdEndpoint.HandleAsync)
            .WithDisplayName("Get Clinic by ID")
            .WithSummary("Retrieves a clinic by its ID.")
            .WithDescription("This endpoint returns the details of a specific clinic identified by its unique ID.");

        _ = group.MapPost(string.Empty, CreateClinicEndpoint.HandleAsync)
            .WithDisplayName("Create Clinic")
            .WithSummary("Creates a new clinic.")
            .WithDescription("This endpoint allows the creation of a new clinic by providing the necessary details in the request body.");

        _ = group.MapPut("{id:guid}", UpdateClinicEndpoint.HandleAsync)
            .WithDisplayName("Update Clinic")
            .WithSummary("Updates an existing clinic.")
            .WithDescription("This endpoint allows updating the details of an existing clinic identified by its unique ID.");

        _ = group.MapDelete("{id:guid}", DeleteClinicEndpoint.HandleAsync)
            .WithDisplayName("Delete Clinic")
            .WithSummary("Deletes a clinic.")
            .WithDescription("This endpoint allows the deletion of a clinic identified by its unique ID.");

        _ = group.MapPost("{id:guid}/veterinarians", AddVeterinarianToClinicEndpoint.HandleAsync)
            .WithDisplayName("Add Veterinarian to Clinic")
            .WithSummary("Adds a veterinarian to a clinic.")
            .WithDescription("This endpoint allows adding a veterinarian to a specific clinic identified by its unique ID.");

        _ = group.MapDelete("{id:guid}/veterinarians/{veterinarianId:guid}", RemoveVeterinarianFromClinicEndpoint.HandleAsync)
            .WithDisplayName("Remove Veterinarian from Clinic")
            .WithSummary("Removes a veterinarian from a clinic.")
            .WithDescription("This endpoint allows removing a veterinarian from a specific clinic identified by its unique ID.");

        return group;
    }
}
