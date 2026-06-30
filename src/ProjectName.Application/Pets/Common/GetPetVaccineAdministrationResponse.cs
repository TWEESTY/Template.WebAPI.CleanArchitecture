namespace ProjectName.Application.Pets.Common;

public sealed record GetPetVaccineAdministrationResponse(
    Guid Id,
    Guid VaccineId,
    Guid VeterinarianId,
    DateOnly AdministrationOn);
