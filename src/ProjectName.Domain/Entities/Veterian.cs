using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents a veterinarian entity in the domain layer, encapsulating the details of a veterinarian, including their name, email, and license number.
/// </summary>
public class Veterinarian : EntityBase
{

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string LicenseNumber { get; private set; } = null!;

    private Veterinarian()
    {
    }

    public Veterinarian(
        string firstName,
        string lastName,
        string email,
        string licenseNumber)
    {
        Id = Guid.NewGuid();

        UpdateProfile(firstName, lastName, email, licenseNumber);

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email,
        string licenseNumber)
    {
        FirstName = Guard.ThrowIfEmptyOrNull(firstName?.Trim(), nameof(FirstName), "First name is required.");
        LastName = Guard.ThrowIfEmptyOrNull(lastName?.Trim(), nameof(LastName), "Last name is required.");
        Email = Guard.ThrowIfEmptyOrNull(email?.Trim(), nameof(Email), "Email is required.");
        LicenseNumber = Guard.ThrowIfEmptyOrNull(licenseNumber?.Trim(), nameof(LicenseNumber), "License number is required.");

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }
}
