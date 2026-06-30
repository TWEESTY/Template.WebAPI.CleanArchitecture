using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Owners.Commands;

public sealed class UpdateOwnerCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task UpdateOwnerCommand_WhenOwnerExists_ShouldUpdateAndReturnOwner()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner createdOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result<GetOwnerResponse> updateResult = await mediator.Send(new UpdateOwnerCommand(
        createdOwner.Id,
        "Updated",
        "Owner",
        $"updated-owner-{Guid.NewGuid():N}@example.test",
        "+1-555-0199"));

        Assert.True(updateResult.IsSuccess, string.Join(" | ", updateResult.Errors.Select(error => $"{error.GetType().Name}: {error.Message}")));
        Assert.Equal("Updated", updateResult.Value.FirstName);
        Assert.Equal("Owner", updateResult.Value.LastName);

        Owner storedOwner = await dbContext.Owners.SingleAsync(owner => owner.Id == createdOwner.Id);
        Assert.Equal("Updated", storedOwner.FirstName);
        Assert.Equal("Owner", storedOwner.LastName);
    }

    [Fact]
    public async Task UpdateOwnerCommand_WhenOwnerDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingOwnerId = Guid.NewGuid();

        Result<GetOwnerResponse> updateResult = await mediator.Send(new UpdateOwnerCommand(
        missingOwnerId,
        "Updated",
        "Owner",
        $"updated-owner-{Guid.NewGuid():N}@example.test",
        "+1-555-0199"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Owners.AnyAsync(owner => owner.Id == missingOwnerId));
    }
}

