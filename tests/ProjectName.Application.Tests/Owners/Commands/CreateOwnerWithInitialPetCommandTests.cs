using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Commands;
using ProjectName.Application.Owners.Queries;
using ProjectName.Application.Owners.Common;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Owners.Commands;

public sealed class CreateOwnerWithInitialPetCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateOwnerWithInitialPetCommand_WhenRequestIsValid_ShouldCreateOwnerAndPet()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        string uniqueEmail = $"owner-with-pet-{Guid.NewGuid():N}@example.test";
        var command = new CreateOwnerWithInitialPetCommand(
            "Bob",
            "Johnson",
            uniqueEmail,
            "+1-555-0101",
            "Milo",
            1,
            fakeTimeProvider.GetUtcNow().AddYears(-2));

        var createResult = await mediator.Send(command);

        Assert.True(createResult.IsSuccess);
        Assert.NotEqual(Guid.Empty, createResult.Value.OwnerId);
        Assert.NotEqual(Guid.Empty, createResult.Value.PetId);

        var ownerResult = await mediator.Send(new GetOwnerByIdQuery(createResult.Value.OwnerId));

        Assert.True(ownerResult.IsSuccess);
        Assert.Equal(createResult.Value.OwnerId, ownerResult.Value.Id);
    }

    [Fact]
    public async Task CreateOwnerWithInitialPetCommand_WhenPetCreationFails_ShouldRollbackOwnerCreation()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        string uniqueEmail = $"owner-with-pet-rollback-{Guid.NewGuid():N}@example.test";
        var command = new CreateOwnerWithInitialPetCommand(
            "Alice",
            "Rollback",
            uniqueEmail,
            "+1-555-0102",
            "Milo",
            999,
            fakeTimeProvider.GetUtcNow().AddYears(-2));

        Result<GetOwnerWithInitialPetResponse> createResult = await mediator.Send(command);

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Owners.AnyAsync(owner => owner.Email == uniqueEmail));
    }
}
