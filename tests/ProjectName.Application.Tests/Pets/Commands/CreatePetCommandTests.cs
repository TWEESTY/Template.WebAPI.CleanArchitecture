using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Pets.Commands;

public sealed class CreatePetCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreatePetCommand_WhenPetIsValid_ShouldCreatePet()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result<GetPetResponse> createResult = await mediator.Send(new CreatePetCommand(owner.Id, "Milo", 1, fakeTimeProvider.GetUtcNow().AddYears(-3)));

        Assert.True(createResult.IsSuccess);
        Assert.Equal("Milo", createResult.Value.Name);
        Pet storedPet = await dbContext.Pets.SingleAsync(pet => pet.Id == createResult.Value.Id);
        Assert.Equal(owner.Id, storedPet.OwnerId);
    }

    [Fact]
    public async Task CreatePetCommand_WhenSpeciesIsInvalid_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result<GetPetResponse> createResult = await mediator.Send(new CreatePetCommand(owner.Id, "Milo", 999, fakeTimeProvider.GetUtcNow().AddYears(-3)));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Pets.AnyAsync(pet => pet.OwnerId == owner.Id && pet.Name == "Milo"));
    }

    [Fact]
    public async Task CreatePetCommand_WhenOwnerIdIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Result<GetPetResponse> createResult = await mediator.Send(new CreatePetCommand(Guid.Empty, "Milo", 1, new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(CreatePetCommand.OwnerId));
    }
}

