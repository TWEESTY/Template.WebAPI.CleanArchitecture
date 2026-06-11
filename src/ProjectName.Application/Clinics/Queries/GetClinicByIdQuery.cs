using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Clinics.Queries;

public sealed record GetClinicByIdQuery(Guid id) : IQuery<Result<GetClinicResponse>>;

public sealed class GetClinicByIdHandler(IClinicRepository clinicRepository) : IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>.Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.id, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.id}' was not found."));
        }

        return Result.Ok(new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address));
    }
}
