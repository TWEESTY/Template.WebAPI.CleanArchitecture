using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Queries;

/// <summary>
/// Represents a query to retrieve a vaccine by its unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the vaccine to retrieve.</param>
public sealed record GetVaccineByIdQuery(Guid Id) : IQuery<Result<GetVaccineResponse>>;

internal sealed class GetVaccineByIdHandler(IVaccineRepository repository) : IQueryHandler<GetVaccineByIdQuery, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> IQueryHandler<GetVaccineByIdQuery, Result<GetVaccineResponse>>.Handle(GetVaccineByIdQuery request, CancellationToken cancellationToken)
    {
        Vaccine? vaccine = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (vaccine is null)
        {
            return Result.Fail(new NotFoundError($"Vaccine '{request.Id}' was not found."));
        }

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
