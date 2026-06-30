using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Pets.Queries;
using ProjectName.Application.Tests.Search;

namespace ProjectName.Application.Tests.Pets.Queries;

public sealed class GetPetsQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetPetsQuery_WhenPetsExist_ShouldReturnCreatedPet()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        var pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);

        var getResult = await mediator.Send(new GetPetsQuery(SearchParameters: null));

        Assert.True(getResult.IsSuccess);
        Assert.Contains(getResult.Value, p => p.Id == pet.Id && p.Name == pet.Name && p.SpecieId == (int)pet.Species);
    }

    [Fact]
    public async Task GetPetsQuery_WhenPageSizeIsOutOfRange_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await SearchParametersValidationTestHelper.AssertInvalidPageSizeAsync(mediator, searchParameters => new GetPetsQuery(searchParameters));
    }
}

