namespace ProjectName.Application.Owners.Common;

public sealed record GetOwnerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber);
