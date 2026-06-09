using ProjectName.Domain.Common.Exceptions;

namespace ProjectName.Domain.Entities;

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

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateContactInformation(
        string firstName,
        string lastName,
        string email,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}