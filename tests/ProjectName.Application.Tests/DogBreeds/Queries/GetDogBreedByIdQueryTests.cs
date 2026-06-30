using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.DogBreeds.Queries;

namespace ProjectName.Application.Tests.DogBreeds.Queries;

public sealed class GetDogBreedByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    private static readonly Guid ExistingBreedId = Guid.Parse("00000000-0000-0000-0000-00000000002a");
    private static readonly Guid MissingBreedId = Guid.Parse("00000000-0000-0000-0000-000000000194");

    [Fact]
    public async Task GetDogBreedByIdQuery_WhenBreedExists_ShouldReturnBreed()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetDogBreedByIdQuery(ExistingBreedId));

        Assert.True(result.IsSuccess);
        Assert.Equal("Test Breed", result.Value.Name);
    }

    [Fact]
    public async Task GetDogBreedByIdQuery_WhenBreedDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetDogBreedByIdQuery(MissingBreedId));

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is NotFoundError);
    }
}
