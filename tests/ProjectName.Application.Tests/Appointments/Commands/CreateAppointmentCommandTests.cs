using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using FluentResults;
using ProjectName.Application.Appointments.Commands;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Appointments.Commands;

public sealed class CreateAppointmentCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateAppointmentCommand_WhenAppointmentIsValid_ShouldCreateAppointment()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        DateTime startAt = fakeTimeProvider.GetUtcNow().UtcDateTime.AddDays(1);
        DateTime endAt = startAt.AddMinutes(45);

        Result<GetAppointmentResponse> createResult = await mediator.Send(new CreateAppointmentCommand(
            pet.Id,
            veterinarian.Id,
            clinic.Id,
            startAt,
            endAt,
            "Annual exam"));

        Assert.True(createResult.IsSuccess);
        Assert.Equal("Scheduled", createResult.Value.Status);

        Appointment storedAppointment = await dbContext.Appointments.SingleAsync(appointment => appointment.Id == createResult.Value.Id);
        Assert.Equal(pet.Id, storedAppointment.PetId);
        Assert.Equal(veterinarian.Id, storedAppointment.VeterinarianId);
        Assert.Equal(clinic.Id, storedAppointment.ClinicId);
    }

    [Fact]
    public async Task CreateAppointmentCommand_WhenEndDateIsBeforeStartDate_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        DateTime startAt = fakeTimeProvider.GetUtcNow().UtcDateTime.AddDays(1);
        DateTime endAt = startAt.AddMinutes(-30);

        Result<GetAppointmentResponse> createResult = await mediator.Send(new CreateAppointmentCommand(
            pet.Id,
            veterinarian.Id,
            clinic.Id,
            startAt,
            endAt,
            "Annual exam"));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Appointments.AnyAsync(appointment => appointment.Reason == "Annual exam" && appointment.PetId == pet.Id));
    }
}

