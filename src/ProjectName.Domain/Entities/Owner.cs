using ProjectName.Domain.Common.Guards;

namespace ProjectName.Domain.Entities;

/// <summary>
/// Represents an owner entity in the domain layer, encapsulating the details of a pet owner, including their contact information and methods to update it.
/// </summary>
public class Owner : EntityBase
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    private Owner()
    {
    }

    public Owner(
        string firstName,
        string lastName,
        string email,
        string phoneNumber)
    {
        Id = Guid.NewGuid();

        UpdateContactInformation(
            firstName,
            lastName,
            email,
            phoneNumber);

        CreatedAt = TimeProvider.System.GetUtcNow();
    }

    public void UpdateContactInformation(
        string firstName,
        string lastName,
        string email,
        string phoneNumber)
    {
        FirstName = Guard.ThrowIfEmptyOrNull(firstName?.Trim(), nameof(FirstName), "First name is required.");
        LastName = Guard.ThrowIfEmptyOrNull(lastName?.Trim(), nameof(LastName), "Last name is required.");
        Email = email;
        PhoneNumber = phoneNumber;

        UpdatedAt = TimeProvider.System.GetUtcNow();
    }
}
