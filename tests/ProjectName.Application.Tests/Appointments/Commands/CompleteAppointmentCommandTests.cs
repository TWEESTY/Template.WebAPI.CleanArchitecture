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

public sealed class CompleteAppointmentCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CompleteAppointmentCommand_WhenAppointmentExists_ShouldMarkAsCompleted()
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

        Result<GetAppointmentResponse> completeResult = await mediator.Send(new CompleteAppointmentCommand(appointment.Id));

        Assert.True(completeResult.IsSuccess);
        Assert.Equal("Completed", completeResult.Value.Status);

        Appointment storedAppointment = await dbContext.Appointments.SingleAsync(item => item.Id == appointment.Id);
        Assert.Equal("Completed", storedAppointment.Status.ToString());
    }

    [Fact]
    public async Task CompleteAppointmentCommand_WhenAppointmentDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingAppointmentId = Guid.NewGuid();

        Result<GetAppointmentResponse> completeResult = await mediator.Send(new CompleteAppointmentCommand(missingAppointmentId));

        Assert.True(completeResult.IsFailed);
        Assert.Contains(completeResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Appointments.AnyAsync(item => item.Id == missingAppointmentId));
    }
}

