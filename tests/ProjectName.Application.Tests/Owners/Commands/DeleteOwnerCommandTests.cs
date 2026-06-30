using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Commands;
using ProjectName.Application.Owners.Queries;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Owners.Commands;

public sealed class DeleteOwnerCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task DeleteOwnerCommand_WhenOwnerExists_ShouldDeleteOwner()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner createdOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);

        Result deleteResult = await mediator.Send(new DeleteOwnerCommand(createdOwner.Id));
        Result<GetOwnerResponse> getByIdResult = await mediator.Send(new GetOwnerByIdQuery(createdOwner.Id));

        Assert.True(deleteResult.IsSuccess);
        Assert.True(getByIdResult.IsFailed);
        Assert.Contains(getByIdResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Owners.AnyAsync(owner => owner.Id == createdOwner.Id));
    }

    [Fact]
    public async Task DeleteOwnerCommand_WhenOwnerDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result deleteResult = await mediator.Send(new DeleteOwnerCommand(Guid.NewGuid()));

        Assert.True(deleteResult.IsFailed);
        Assert.Contains(deleteResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Owners.AnyAsync());
    }
}

