using Bogus;
using Microsoft.EntityFrameworkCore;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence;

public sealed class ApplicationDbContextInitialiser(AppDbContext context)
{
    private const int OwnerCount = 80;
    private const int VeterinarianCount = 35;
    private const int ClinicCount = 20;
    private const int VaccineCount = 40;

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);
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

        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<Owner> GenerateOwners()
    {
        var ownerFaker = new Faker<Owner>()
            .CustomInstantiator(f =>
                new Owner(
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Internet.Email(),
                    f.Phone.PhoneNumber("+1-###-###-####")));

        return ownerFaker.Generate(OwnerCount);
    }

    private static List<Veterinarian> GenerateVeterinarians()
    {
        var vetFaker = new Faker<Veterinarian>()
            .CustomInstantiator(f =>
                new Veterinarian(
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Internet.Email(),
                    licenseNumber: $"VET-{f.Random.Number(100000, 999999)}"));

        return vetFaker.Generate(VeterinarianCount);
    }

    private static List<Clinic> GenerateClinics()
    {
        var clinicFaker = new Faker<Clinic>()
            .CustomInstantiator(f =>
                new Clinic(
                    $"{f.Company.CompanyName()} Veterinary Clinic",
                    f.Address.FullAddress()));

        return clinicFaker.Generate(ClinicCount);
    }

    private static List<ClinicVeterinarian> GenerateClinicVeterinarians(
        IReadOnlyList<Clinic> clinics,
        IReadOnlyList<Veterinarian> veterinarians)
    {
        var random = new Randomizer();
        var links = new List<ClinicVeterinarian>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (Clinic clinic in clinics)
        {
            int vetCountForClinic = random.Int(4, 10);
            List<Veterinarian> selectedVets = veterinarians
                .OrderBy(_ => random.Int())
                .Take(vetCountForClinic)
                .ToList();

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

        var vaccineFaker = new Faker<Vaccine>()
            .CustomInstantiator(f =>
                new Vaccine(
                    $"{f.PickRandom(vaccinePrefixes)}-{f.Random.Number(100, 999)}",
                    $"{f.Commerce.ProductAdjective()} {f.Commerce.Product()} Vaccine"));

        return vaccineFaker.Generate(VaccineCount);
    }

    private static List<Pet> GeneratePets(
        IReadOnlyList<Owner> owners,
        IReadOnlyList<Vaccine> vaccines,
        IReadOnlyList<Veterinarian> veterinarians)
    {
        var random = new Randomizer();
        var pets = new List<Pet>();

        foreach (Owner owner in owners)
        {
            int petCount = random.Int(1, 4);
            for (int i = 0; i < petCount; i++)
            {
                PetSpecies species = PickSpecies(random.Int(1, 5));
                DateOnly birthDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-random.Int(200, 4500)));

                Pet pet = new(
                    owner.Id,
                    new Faker().Name.FirstName(),
                    species,
                    birthDate);

                int vaccinationCount = random.Int(1, 4);
                List<Vaccine> selectedVaccines = vaccines
                    .OrderBy(_ => random.Int())
                    .Take(vaccinationCount)
                    .ToList();

                foreach (Vaccine vaccine in selectedVaccines)
                {
                    DateOnly administrationDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-random.Int(1, 720)));
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
        var random = new Randomizer();
        var appointments = new List<Appointment>();

        int appointmentTarget = Math.Max(250, pets.Count * 2);

        for (int i = 0; i < appointmentTarget; i++)
        {
            Pet pet = pets[random.Int(0, pets.Count - 1)];
            Clinic clinic = clinics[random.Int(0, clinics.Count - 1)];
            Veterinarian veterinarian = veterinarians[random.Int(0, veterinarians.Count - 1)];

            DateTime start = DateTime.UtcNow.Date
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

            if (end < DateTime.UtcNow && random.Bool(0.75f))
            {
                appointment.Complete();
            }
            else if (start > DateTime.UtcNow && random.Bool(0.15f))
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
