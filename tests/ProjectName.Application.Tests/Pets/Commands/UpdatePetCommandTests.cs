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

public sealed class UpdatePetCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task UpdatePetCommand_WhenPetExists_ShouldUpdatePet()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result<GetPetResponse> updateResult = await mediator.Send(new UpdatePetCommand(pet.Id, "Nova", fakeTimeProvider.GetUtcNow().AddYears(-1)));

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Nova", updateResult.Value.Name);
        Pet storedPet = await dbContext.Pets.SingleAsync(item => item.Id == pet.Id);
        Assert.Equal("Nova", storedPet.Name);
    }

    [Fact]
    public async Task UpdatePetCommand_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingPetId = Guid.NewGuid();
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result<GetPetResponse> updateResult = await mediator.Send(new UpdatePetCommand(missingPetId, "Nova", fakeTimeProvider.GetUtcNow().AddYears(-1)));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Pets.AnyAsync(item => item.Id == missingPetId));
    }

    [Fact]
    public async Task UpdatePetCommand_WhenNameIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result<GetPetResponse> updateResult = await mediator.Send(new UpdatePetCommand(Guid.NewGuid(), string.Empty, fakeTimeProvider.GetUtcNow().AddYears(-1)));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(UpdatePetCommand.Name));
    }
}

