using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Clinics.Commands;

public sealed class RemoveVeterinarianFromClinicCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task RemoveVeterinarianFromClinicCommand_WhenVeterinarianWasPreviouslyAdded_ShouldRemoveVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        dbContext.ChangeTracker.Clear();
        _ = await mediator.Send(new AddVeterinarianToClinicCommand(clinic.Id, veterinarian.Id));
        dbContext.ChangeTracker.Clear();

        Result removeResult = await mediator.Send(new RemoveVeterinarianFromClinicCommand(clinic.Id, veterinarian.Id));

        Assert.True(removeResult.IsSuccess);
        Assert.False(await dbContext.ClinicVeterinarians.AnyAsync(item => item.ClinicId == clinic.Id && item.VeterinarianId == veterinarian.Id));
    }

    [Fact]
    public async Task RemoveVeterinarianFromClinicCommand_WhenClinicDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingClinicId = Guid.NewGuid();
        Guid missingVeterinarianId = Guid.NewGuid();

        Result removeResult = await mediator.Send(new RemoveVeterinarianFromClinicCommand(missingClinicId, missingVeterinarianId));

        Assert.True(removeResult.IsFailed);
        Assert.Contains(removeResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.ClinicVeterinarians.AnyAsync(item => item.ClinicId == missingClinicId && item.VeterinarianId == missingVeterinarianId));
    }
}

