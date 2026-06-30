using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Queries;

namespace ProjectName.Application.Tests.Pets.Queries;

public sealed class GetPetQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetPetQuery_WhenPetExists_ShouldReturnPet()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        var pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);

        var getResult = await mediator.Send(new GetPetQuery(pet.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(pet.Id, getResult.Value.Id);
    }

    [Fact]
    public async Task GetPetQuery_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetPetQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

