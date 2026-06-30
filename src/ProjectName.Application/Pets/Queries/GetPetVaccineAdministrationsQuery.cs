using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Pets.Queries;

/// <summary>
/// Represents a query to retrieve a list of vaccine administrations for a specific pet in the application.
/// </summary>
/// <param name="PetId">The unique identifier of the pet.</param>
public sealed record GetPetVaccineAdministrationsQuery(Guid PetId) : IQuery<Result<List<GetPetVaccineAdministrationResponse>>>;

internal sealed class GetPetVaccineAdministrationsHandler(IPetRepository petRepository) : IQueryHandler<GetPetVaccineAdministrationsQuery, Result<List<GetPetVaccineAdministrationResponse>>>
{
    async ValueTask<Result<List<GetPetVaccineAdministrationResponse>>> IQueryHandler<GetPetVaccineAdministrationsQuery, Result<List<GetPetVaccineAdministrationResponse>>>.Handle(GetPetVaccineAdministrationsQuery request, CancellationToken cancellationToken)
    {
        Pet? pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.PetId}' was not found."));
        }

        List<GetPetVaccineAdministrationResponse> responses = [.. pet.VaccineAdministrations.Select(v => new GetPetVaccineAdministrationResponse(v.Id, v.VaccineId, v.VeterinarianId, v.AdministrationOn))];

        return Result.Ok(responses);
    }
}
