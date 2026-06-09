using Microsoft.AspNetCore.Mvc;
using ProjectName.Web.Api.Clinics.Commands;
using ProjectName.Web.Api.Clinics.Queries;
using ProjectName.Web.Api.Common.Search;

namespace ProjectName.Web.Api.Clinics;

public static class ClinicsEndpointsGroup
{
    public const string BasePath = "/api/clinics";
    public const string GroupName = "Clinics";

    public static RouteGroupBuilder MapClinicsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, ([FromQuery] SearchParameters searchParameters) => GetClinicsEndpoint.HandleAsync(searchParameters))
            .WithDisplayName("Get Clinics")
            .WithSummary("Retrieves a list of clinics.")
            .WithDescription("This endpoint returns a list of all clinics in the system.");

        group.MapGet("{id:guid}", ([FromRoute] Guid id) => GetClinicByIdEndpoint.HandleAsync(id))
            .WithDisplayName("Get Clinic by ID")
            .WithSummary("Retrieves a clinic by its ID.")
            .WithDescription("This endpoint returns the details of a specific clinic identified by its unique ID.");

        group.MapPost(string.Empty, ([FromBody] CreateClinicEndpoint.CreateClinicRequest request) => CreateClinicEndpoint.HandleAsync(request))
            .WithDisplayName("Create Clinic")
            .WithSummary("Creates a new clinic.")
            .WithDescription("This endpoint allows the creation of a new clinic by providing the necessary details in the request body.");

        group.MapPut("{id:guid}", ([FromRoute] Guid id, [FromBody] UpdateClinicEndpoint.UpdateClinicRequest request) => UpdateClinicEndpoint.HandleAsync(id, request))
            .WithDisplayName("Update Clinic")
            .WithSummary("Updates an existing clinic.")
            .WithDescription("This endpoint allows updating the details of an existing clinic identified by its unique ID.");

        group.MapDelete("{id:guid}", ([FromRoute] Guid id) => DeleteClinicEndpoint.HandleAsync(id))
            .WithDisplayName("Delete Clinic")
            .WithSummary("Deletes a clinic.")
            .WithDescription("This endpoint allows the deletion of a clinic identified by its unique ID.");

        group.MapPost("{id:guid}/veterinarians", ([FromRoute] Guid id, [FromBody] AddVeterinarianToClinicEndpoint.AddVeterinarianToClinicRequest request) => AddVeterinarianToClinicEndpoint.HandleAsync(id, request))
            .WithDisplayName("Add Veterinarian to Clinic")
            .WithSummary("Adds a veterinarian to a clinic.")
            .WithDescription("This endpoint allows adding a veterinarian to a specific clinic identified by its unique ID.");

        group.MapDelete("{id:guid}/veterinarians/{veterinarianId:guid}", ([FromRoute] Guid id, [FromRoute] Guid veterinarianId) => RemoveVeterinarianFromClinicEndpoint.HandleAsync(id, veterinarianId))
            .WithDisplayName("Remove Veterinarian from Clinic")
            .WithSummary("Removes a veterinarian from a clinic.")
            .WithDescription("This endpoint allows removing a veterinarian from a specific clinic identified by its unique ID.");

        return group;
    }
}
