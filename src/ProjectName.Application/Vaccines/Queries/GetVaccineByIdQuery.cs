using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Queries;

public sealed record GetVaccineByIdQuery(Guid Id) : IQuery<Result<GetVaccineResponse>>;

public sealed class GetVaccineByIdHandler(IVaccineRepository repository) : IQueryHandler<GetVaccineByIdQuery, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> IQueryHandler<GetVaccineByIdQuery, Result<GetVaccineResponse>>.Handle(GetVaccineByIdQuery request, CancellationToken cancellationToken)
    {
        var vaccine = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (vaccine is null)
        {
            return Result.Fail(new NotFoundError($"Vaccine '{request.Id}' was not found."));
        }

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
