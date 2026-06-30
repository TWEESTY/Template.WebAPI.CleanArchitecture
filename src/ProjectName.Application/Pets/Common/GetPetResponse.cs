namespace ProjectName.Application.Pets.Common;

public record GetPetResponse(Guid Id, string Name, DateTimeOffset BirthDate, int SpecieId);
