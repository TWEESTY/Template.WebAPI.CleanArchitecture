namespace ProjectName.Application.Owners.Common;

public sealed record GetOwnerWithInitialPetResponse(
    Guid OwnerId,
    string OwnerFirstName,
    string OwnerLastName,
    Guid PetId,
    string PetName,
    DateTimeOffset PetBirthDate);