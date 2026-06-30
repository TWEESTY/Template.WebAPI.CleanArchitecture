using Bogus;
using Microsoft.EntityFrameworkCore;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence;

/// <summary>
/// Represents the initializer for the application's database context, responsible for ensuring the database is created and seeding initial data for owners, veterinarians, clinics, clinic-veterinarian relationships, vaccines, pets, and appointments.
/// </summary>
/// <param name="context">The application's database context.</param>
public sealed class ApplicationDbContextInitialiser(AppDbContext context)
{
    private const int _ownerCount = 80;
    private const int _veterinarianCount = 35;
    private const int _clinicCount = 20;
    private const int _vaccineCount = 40;

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        _ = await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.Owners.AnyAsync(cancellationToken))
        {
            return;
        }

        Randomizer.Seed = new Random(7391);

        List<Owner> owners = GenerateOwners();
        List<Veterinarian> veterinarians = GenerateVeterinarians();
        List<Clinic> clinics = GenerateClinics();
        List<ClinicVeterinarian> clinicVeterinarians = GenerateClinicVeterinarians(clinics, veterinarians);
        List<Vaccine> vaccines = GenerateVaccines();
        List<Pet> pets = GeneratePets(owners, vaccines, veterinarians);
        List<Appointment> appointments = GenerateAppointments(pets, clinics, veterinarians);

        await context.Owners.AddRangeAsync(owners, cancellationToken);
        await context.Veterinarians.AddRangeAsync(veterinarians, cancellationToken);
        await context.Clinics.AddRangeAsync(clinics, cancellationToken);
        await context.ClinicVeterinarians.AddRangeAsync(clinicVeterinarians, cancellationToken);
        await context.Vaccines.AddRangeAsync(vaccines, cancellationToken);
        await context.Pets.AddRangeAsync(pets, cancellationToken);
        await context.Appointments.AddRangeAsync(appointments, cancellationToken);

        _ = await context.SaveChangesAsync(cancellationToken);
    }

    private static List<Owner> GenerateOwners()
    {
        Faker<Owner> ownerFaker = new Faker<Owner>()
            .CustomInstantiator(f =>
                new Owner(
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Internet.Email(),
                    f.Phone.PhoneNumber("+1-###-###-####")));

        return ownerFaker.Generate(_ownerCount);
    }

    private static List<Veterinarian> GenerateVeterinarians()
    {
        Faker<Veterinarian> vetFaker = new Faker<Veterinarian>()
            .CustomInstantiator(f =>
                new Veterinarian(
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Internet.Email(),
                    licenseNumber: $"VET-{f.Random.Number(100000, 999999)}"));

        return vetFaker.Generate(_veterinarianCount);
    }

    private static List<Clinic> GenerateClinics()
    {
        Faker<Clinic> clinicFaker = new Faker<Clinic>()
            .CustomInstantiator(f =>
                new Clinic(
                    $"{f.Company.CompanyName()} Veterinary Clinic",
                    f.Address.FullAddress()));

        return clinicFaker.Generate(_clinicCount);
    }

    private static List<ClinicVeterinarian> GenerateClinicVeterinarians(
        IReadOnlyList<Clinic> clinics,
        IReadOnlyList<Veterinarian> veterinarians)
    {
        Randomizer random = new();
        List<ClinicVeterinarian> links = [];
        HashSet<string> seen = new(StringComparer.Ordinal);

        foreach (Clinic clinic in clinics)
        {
            int vetCountForClinic = random.Int(4, 10);
            List<Veterinarian> selectedVets = [.. veterinarians
                .OrderBy(_ => random.Int())
                .Take(vetCountForClinic)];

            foreach (Veterinarian vet in selectedVets)
            {
                string key = $"{clinic.Id:N}:{vet.Id:N}";
                if (seen.Add(key))
                {
                    links.Add(new ClinicVeterinarian(clinic.Id, vet.Id));
                }
            }
        }

        return links;
    }

    private static List<Vaccine> GenerateVaccines()
    {
        string[] vaccinePrefixes = ["RAB", "DHP", "FVR", "BRC", "LYM", "PAR", "LEP", "CIV"];

        Faker<Vaccine> vaccineFaker = new Faker<Vaccine>()
            .CustomInstantiator(f =>
                new Vaccine(
                    $"{f.PickRandom(vaccinePrefixes)}-{f.Random.Number(100, 999)}",
                    $"{f.Commerce.ProductAdjective()} {f.Commerce.Product()} Vaccine"));

        return vaccineFaker.Generate(_vaccineCount);
    }

    private static List<Pet> GeneratePets(
        IReadOnlyList<Owner> owners,
        IReadOnlyList<Vaccine> vaccines,
        IReadOnlyList<Veterinarian> veterinarians)
    {
        Randomizer random = new();
        List<Pet> pets = [];

        foreach (Owner owner in owners)
        {
            int petCount = random.Int(1, 4);
            for (int i = 0; i < petCount; i++)
            {
                PetSpecies species = PickSpecies(random.Int(1, 5));
                DateTime currentUtc = TimeProvider.System.GetUtcNow().UtcDateTime;
                DateOnly birthDate = DateOnly.FromDateTime(currentUtc.Date.AddDays(-random.Int(200, 4500)));

                Pet pet = new(
                    owner.Id,
                    new Faker().Name.FirstName(),
                    species,
                    birthDate);

                int vaccinationCount = random.Int(1, 4);
                List<Vaccine> selectedVaccines = [.. vaccines
                    .OrderBy(_ => random.Int())
                    .Take(vaccinationCount)];

                foreach (Vaccine vaccine in selectedVaccines)
                {
                    DateOnly administrationDate = DateOnly.FromDateTime(currentUtc.Date.AddDays(-random.Int(1, 720)));
                    Guid veterinarianId = veterinarians[random.Int(0, veterinarians.Count - 1)].Id;

                    pet.AddVaccineAdministration(vaccine.Id, veterinarianId, administrationDate);
                }

                pets.Add(pet);
            }
        }

        return pets;
    }

    private static List<Appointment> GenerateAppointments(
        IReadOnlyList<Pet> pets,
        IReadOnlyList<Clinic> clinics,
        IReadOnlyList<Veterinarian> veterinarians)
    {
        Randomizer random = new();
        List<Appointment> appointments = [];

        int appointmentTarget = Math.Max(250, pets.Count * 2);

        for (int i = 0; i < appointmentTarget; i++)
        {
            Pet pet = pets[random.Int(0, pets.Count - 1)];
            Clinic clinic = clinics[random.Int(0, clinics.Count - 1)];
            Veterinarian veterinarian = veterinarians[random.Int(0, veterinarians.Count - 1)];

            DateTime nowUtc = TimeProvider.System.GetUtcNow().UtcDateTime;

            DateTime start = nowUtc.Date
                .AddDays(random.Int(-120, 120))
                .AddHours(random.Int(8, 17))
                .AddMinutes(random.Int(0, 3) * 15);

            DateTime end = start.AddMinutes(random.Int(20, 90));

            Appointment appointment = new(
                pet.Id,
                veterinarian.Id,
                clinic.Id,
                start,
                end,
                new Faker().Lorem.Sentence(8));

            if (end < nowUtc && random.Bool(0.75f))
            {
                appointment.Complete();
            }
            else if (start > nowUtc && random.Bool(0.15f))
            {
                appointment.Cancel();
            }

            appointments.Add(appointment);
        }

        return appointments;
    }

    private static PetSpecies PickSpecies(int value)
    {
        return value switch
        {
            1 => PetSpecies.Dog,
            2 => PetSpecies.Cat,
            3 => PetSpecies.Rabbit,
            4 => PetSpecies.Hamster,
            _ => PetSpecies.GuineaPig
        };
    }
}
