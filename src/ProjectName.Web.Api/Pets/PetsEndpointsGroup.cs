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

        return group;
    }
}