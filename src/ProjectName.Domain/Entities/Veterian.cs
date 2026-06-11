namespace ProjectName.Domain.Entities;

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

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email,
        string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Common.Exceptions.DomainException("First name is required.", nameof(FirstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new Common.Exceptions.DomainException("Last name is required.", nameof(LastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new Common.Exceptions.DomainException("Email is required.", nameof(Email));

        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new Common.Exceptions.DomainException("License number is required.", nameof(LicenseNumber));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        LicenseNumber = licenseNumber.Trim();

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}