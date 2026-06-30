using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Queries;

/// <summary>
/// Represents a query to retrieve a clinic by its unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the clinic.</param>
public sealed record GetClinicByIdQuery(Guid Id) : IQuery<Result<GetClinicResponse>>;

internal sealed class GetClinicByIdHandler(IClinicRepository clinicRepository) : IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>
{
    async ValueTask<Result<GetClinicResponse>> IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>.Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
    {
        Clinic? clinic = await clinicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (clinic is null)
        {
            return Result.Fail(new NotFoundError($"Clinic '{request.Id}' was not found."));
        }

        return Result.Ok(new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address));
    }
}
