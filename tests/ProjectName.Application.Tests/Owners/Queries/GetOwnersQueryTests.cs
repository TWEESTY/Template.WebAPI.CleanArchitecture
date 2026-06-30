using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Owners.Queries;
using ProjectName.Application.Tests.Search;

namespace ProjectName.Application.Tests.Owners.Queries;

public sealed class GetOwnersQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetOwnersQuery_WhenOwnersExist_ShouldReturnCreatedOwner()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var createdOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);

        var ownersResult = await mediator.Send(new GetOwnersQuery(SearchParameters: null));

        Assert.True(ownersResult.IsSuccess);
        Assert.Contains(ownersResult.Value, owner =>
            owner.Id == createdOwner.Id &&
            owner.FirstName == createdOwner.FirstName &&
            owner.LastName == createdOwner.LastName &&
            owner.Email == createdOwner.Email);
    }

    [Fact]
    public async Task GetOwnersQuery_WhenPageNumberIsZero_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await SearchParametersValidationTestHelper.AssertInvalidPageNumberAsync(mediator, searchParameters => new GetOwnersQuery(searchParameters));
    }
}

