using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Queries;

namespace ProjectName.Application.Tests.Owners.Queries;

public sealed class GetOwnerByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetOwnerByIdQuery_WhenOwnerExists_ShouldReturnOwner()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var createdOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);

        var getByIdResult = await mediator.Send(new GetOwnerByIdQuery(createdOwner.Id));

        Assert.True(getByIdResult.IsSuccess);
        Assert.Equal(createdOwner.Id, getByIdResult.Value.Id);
    }

    [Fact]
    public async Task GetOwnerByIdQuery_WhenOwnerDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getByIdResult = await mediator.Send(new GetOwnerByIdQuery(Guid.NewGuid()));

        Assert.True(getByIdResult.IsFailed);
        Assert.Contains(getByIdResult.Errors, error => error is NotFoundError);
    }
}

