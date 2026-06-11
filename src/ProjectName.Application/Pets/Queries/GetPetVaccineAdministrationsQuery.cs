using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Queries;

public sealed record GetPetVaccineAdministrationsQuery(Guid PetId) : IQuery<Result<List<GetPetVaccineAdministrationResponse>>>;

public sealed class GetPetVaccineAdministrationsHandler(IPetRepository petRepository) : IQueryHandler<GetPetVaccineAdministrationsQuery, Result<List<GetPetVaccineAdministrationResponse>>>
{
    async ValueTask<Result<List<GetPetVaccineAdministrationResponse>>> IQueryHandler<GetPetVaccineAdministrationsQuery, Result<List<GetPetVaccineAdministrationResponse>>>.Handle(GetPetVaccineAdministrationsQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        List<GetPetVaccineAdministrationResponse> responses = pet.VaccineAdministrations
            .Select(v => new GetPetVaccineAdministrationResponse(v.Id, v.VaccineId, v.VeterinarianId, v.AdministrationOn))
            .ToList();

        return Result.Ok(responses);
    }
}
