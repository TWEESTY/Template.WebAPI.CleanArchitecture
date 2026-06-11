using ProjectName.Web.Api.Pets.Commands;
using ProjectName.Web.Api.Pets.Queries;

namespace ProjectName.Web.Api.Pets;

public static class PetsEndpointsGroup
{
    public const string BasePath = "/api/pets";
    public const string GroupName = "Pets";

    public static RouteGroupBuilder MapPetsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, GetPetsEndpoint.HandleAsync)
            .WithDisplayName("Get Pets")
            .WithSummary("Retrieves a list of pets.")
            .WithDescription("This endpoint returns a list of pets based on the provided search parameters.");

        group.MapGet("{id:guid}", GetPetEndpoint.HandleAsync)
            .WithDisplayName("Get Pet by ID")
            .WithSummary("Retrieves a pet by its ID.")
            .WithDescription("This endpoint returns the details of a pet identified by the provided ID.");

        group.MapPost(string.Empty, CreatePetEndpoint.HandleAsync)
            .WithDisplayName("Create Pet")
            .WithSummary("Creates a new pet.")
            .WithDescription("This endpoint creates a new pet with the provided information.");

        group.MapPut("{id:guid}", UpdatePetEndpoint.HandleAsync)
            .WithDisplayName("Update Pet")
            .WithSummary("Updates an existing pet.")
            .WithDescription("This endpoint updates the information of an existing pet identified by the provided ID.");

        group.MapDelete("{id:guid}", DeletePetEndpoint.HandleAsync)
            .WithDisplayName("Delete Pet")
            .WithSummary("Deletes an existing pet.")
            .WithDescription("This endpoint deletes an existing pet identified by the provided ID.");

        group.MapPost("{id:guid}/transfer-ownership", TransferPetOwnershipEndpoint.HandleAsync)
            .WithDisplayName("Transfer Pet Ownership")
            .WithSummary("Transfers a pet to a new owner.")
            .WithDescription("This endpoint transfers ownership of a pet to another owner.");

        group.MapGet("{id:guid}/vaccine-administrations", GetPetVaccineAdministrationsEndpoint.HandleAsync)
            .WithDisplayName("Get Pet Vaccine Administrations")
            .WithSummary("Retrieves vaccine administrations for a pet.")
            .WithDescription("This endpoint returns all vaccine administrations for the specified pet.");

        group.MapPost("{id:guid}/vaccine-administrations", AddVaccineAdministrationEndpoint.HandleAsync)
            .WithDisplayName("Add Vaccine Administration")
            .WithSummary("Adds a vaccine administration to a pet.")
            .WithDescription("This endpoint records a vaccine administration for the specified pet.");

        group.MapDelete("{id:guid}/vaccine-administrations/{vaccineAdministrationId:guid}", RemoveVaccineAdministrationEndpoint.HandleAsync)
            .WithDisplayName("Remove Vaccine Administration")
            .WithSummary("Removes a vaccine administration from a pet.")
            .WithDescription("This endpoint removes a vaccine administration record for the specified pet.");

        return group;
    }
}