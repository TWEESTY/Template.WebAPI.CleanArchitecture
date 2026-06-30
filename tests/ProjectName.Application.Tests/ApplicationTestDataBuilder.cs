using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;
using ProjectName.Infrastructure.Persistence;
using Microsoft.Extensions.Time.Testing;

namespace ProjectName.Application.Tests;

internal static class ApplicationTestDataBuilder
{
    public static async Task<Owner> CreateOwnerAsync(AppDbContext dbContext)
    {
        var owner = new Owner(
            "Owner",
            $"User{Guid.NewGuid():N}",
            $"owner-{Guid.NewGuid():N}@example.test",
            "+1-555-0100");

        _ = await dbContext.Owners.AddAsync(owner);
        _ = await dbContext.SaveChangesAsync();

        return owner;
    }

    public static async Task<Pet> CreatePetAsync(AppDbContext dbContext, Guid ownerId)
    {
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        DateOnly birthDate = DateOnly.FromDateTime(fakeTimeProvider.GetUtcNow().UtcDateTime.AddYears(-2));

        var pet = new Pet(
            ownerId,
            $"Pet-{Guid.NewGuid():N}",
            PetSpecies.Dog,
            birthDate);

        _ = await dbContext.Pets.AddAsync(pet);
        _ = await dbContext.SaveChangesAsync();

        return pet;
    }

    public static async Task<Clinic> CreateClinicAsync(AppDbContext dbContext)
    {
        var clinic = new Clinic(
            $"Clinic-{Guid.NewGuid():N}",
            "100 Main Street");

        _ = await dbContext.Clinics.AddAsync(clinic);
        _ = await dbContext.SaveChangesAsync();

        return clinic;
    }

    public static async Task<Veterinarian> CreateVeterinarianAsync(AppDbContext dbContext)
    {
        var veterinarian = new Veterinarian(
            "Pat",
            $"Vet{Guid.NewGuid():N}",
            $"vet-{Guid.NewGuid():N}@example.test",
            $"LIC-{Guid.NewGuid():N}");

        _ = await dbContext.Veterinarians.AddAsync(veterinarian);
        _ = await dbContext.SaveChangesAsync();

        return veterinarian;
    }

    public static async Task<Vaccine> CreateVaccineAsync(AppDbContext dbContext)
    {
        var vaccine = new Vaccine(
            $"VX-{Guid.NewGuid():N}"[..10],
            $"Vaccine-{Guid.NewGuid():N}");

        _ = await dbContext.Vaccines.AddAsync(vaccine);
        _ = await dbContext.SaveChangesAsync();

        return vaccine;
    }

    public static async Task<Appointment> CreateAppointmentAsync(AppDbContext dbContext, Guid petId, Guid veterinarianId, Guid clinicId)
    {
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        DateTimeOffset startAt = fakeTimeProvider.GetUtcNow().AddDays(1);
        DateTimeOffset endAt = startAt.AddMinutes(30);

        var appointment = new Appointment(
            petId,
            veterinarianId,
            clinicId,
            startAt,
            endAt,
            "Routine checkup");

        _ = await dbContext.Appointments.AddAsync(appointment);
        _ = await dbContext.SaveChangesAsync();

        return appointment;
    }
}
