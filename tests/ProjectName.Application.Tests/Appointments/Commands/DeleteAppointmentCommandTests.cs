using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Appointments.Commands;

public sealed class DeleteAppointmentCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task DeleteAppointmentCommand_WhenAppointmentExists_ShouldDeleteAppointment()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        Appointment appointment = await ApplicationTestDataBuilder.CreateAppointmentAsync(dbContext, pet.Id, veterinarian.Id, clinic.Id);

        Result deleteResult = await mediator.Send(new DeleteAppointmentCommand(appointment.Id));
        Result<GetAppointmentResponse> getResult = await mediator.Send(new GetAppointmentByIdQuery(appointment.Id));

        Assert.True(deleteResult.IsSuccess);
        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Appointments.AnyAsync(item => item.Id == appointment.Id));
    }

    [Fact]
    public async Task DeleteAppointmentCommand_WhenAppointmentDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result deleteResult = await mediator.Send(new DeleteAppointmentCommand(Guid.NewGuid()));

        Assert.True(deleteResult.IsFailed);
        Assert.Contains(deleteResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Appointments.AnyAsync());
    }
}

