namespace ProjectName.Application.Veterinarians.Common;

public sealed record GetVeterinarianResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string LicenseNumber);
