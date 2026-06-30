using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Clinics.Commands;

public sealed class UpdateClinicCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task UpdateClinicCommand_WhenClinicExists_ShouldUpdateClinic()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result<GetClinicResponse> updateResult = await mediator.Send(new UpdateClinicCommand(clinic.Id, "Renamed Clinic", "300 Main Street"));

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Renamed Clinic", updateResult.Value.Name);
        Clinic storedClinic = await dbContext.Clinics.SingleAsync(item => item.Id == clinic.Id);
        Assert.Equal("Renamed Clinic", storedClinic.Name);
        Assert.Equal("300 Main Street", storedClinic.Address);
    }

    [Fact]
    public async Task UpdateClinicCommand_WhenClinicDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingClinicId = Guid.NewGuid();

        Result<GetClinicResponse> updateResult = await mediator.Send(new UpdateClinicCommand(missingClinicId, "Renamed Clinic", "300 Main Street"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Clinics.AnyAsync(item => item.Id == missingClinicId));
    }
}

