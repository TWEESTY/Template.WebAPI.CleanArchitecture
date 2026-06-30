using ProjectName.Web.Api.DogBreeds.Queries;

namespace ProjectName.Web.Api.DogBreeds;

/// <summary>
/// Represents a group of endpoints related to dog breed information, providing access to dog breed details from external sources. This class provides methods to map the endpoints to the application's routing system and configure their behavior without requiring authorization.
/// </summary>
public static class DogBreedsEndpointsGroup
{
    public const string BasePath = "/api/dog-breeds";
    public const string GroupName = "DogBreeds";

    public static RouteGroupBuilder MapDogBreedsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName);

        _ = group.MapGet("{breedId:guid}", GetDogBreedByIdEndpoint.HandleAsync)
            .WithDisplayName("Get Dog Breed by ID")
            .WithSummary("Retrieves dog breed details from external API.")
            .WithDescription("This endpoint retrieves detailed information about a dog breed using the provided breed ID. Example: /api/dog-breeds/036feed0-da8a-42c9-ab9a-57449b530b13")
            .AllowAnonymous();

        return group;
    }
}
