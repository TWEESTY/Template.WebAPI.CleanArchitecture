using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Appointments.Commands;

public sealed class CancelAppointmentCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CancelAppointmentCommand_WhenAppointmentExists_ShouldMarkAsCancelled()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        Appointment appointment = await ApplicationTestDataBuilder.CreateAppointmentAsync(dbContext, pet.Id, veterinarian.Id, clinic.Id);
        dbContext.ChangeTracker.Clear();

        Result<GetAppointmentResponse> cancelResult = await mediator.Send(new CancelAppointmentCommand(appointment.Id));

        Assert.True(cancelResult.IsSuccess);
        Assert.Equal("Cancelled", cancelResult.Value.Status);

        Appointment storedAppointment = await dbContext.Appointments.SingleAsync(item => item.Id == appointment.Id);
        Assert.Equal("Cancelled", storedAppointment.Status.ToString());
    }

    [Fact]
    public async Task CancelAppointmentCommand_WhenAppointmentDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result<GetAppointmentResponse> cancelResult = await mediator.Send(new CancelAppointmentCommand(Guid.NewGuid()));

        Assert.True(cancelResult.IsFailed);
        Assert.Contains(cancelResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Appointments.AnyAsync());
    }
}

