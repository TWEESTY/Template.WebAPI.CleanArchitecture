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

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        LicenseNumber = licenseNumber;

        CreatedAt = DateTimeOffset.UtcNow;
    }
}