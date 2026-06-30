using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Veterinarians.Commands;

public sealed class UpdateVeterinarianCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task UpdateVeterinarianCommand_WhenVeterinarianExists_ShouldUpdateVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result<GetVeterinarianResponse> updateResult = await mediator.Send(new UpdateVeterinarianCommand(
            veterinarian.Id,
            "Updated",
            "Vet",
            $"updated-vet-{Guid.NewGuid():N}@example.test",
            $"UPDATED-{Guid.NewGuid():N}"));

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Updated", updateResult.Value.FirstName);
        Veterinarian storedVeterinarian = await dbContext.Veterinarians.SingleAsync(item => item.Id == veterinarian.Id);
        Assert.Equal("Updated", storedVeterinarian.FirstName);
    }

    [Fact]
    public async Task UpdateVeterinarianCommand_WhenVeterinarianDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingVeterinarianId = Guid.NewGuid();

        Result<GetVeterinarianResponse> updateResult = await mediator.Send(new UpdateVeterinarianCommand(
            missingVeterinarianId,
            "Updated",
            "Vet",
            $"updated-vet-{Guid.NewGuid():N}@example.test",
            $"UPDATED-{Guid.NewGuid():N}"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Veterinarians.AnyAsync(item => item.Id == missingVeterinarianId));
    }

    [Fact]
    public async Task UpdateVeterinarianCommand_WhenIdIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Result<GetVeterinarianResponse> updateResult = await mediator.Send(new UpdateVeterinarianCommand(
            Guid.Empty,
            "Updated",
            "Vet",
            $"updated-vet-{Guid.NewGuid():N}@example.test",
            $"UPDATED-{Guid.NewGuid():N}"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(UpdateVeterinarianCommand.Id));
    }
}

