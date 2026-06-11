using FluentResults;
using Mediator;
using ProjectName.Application.Clinics.Common;

namespace ProjectName.Application.Clinics.Queries;

public sealed record GetClinicByIdQuery(Guid id) : IQuery<Result<GetClinicResponse>>;

public sealed class GetClinicByIdHandler : IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>
{
    ValueTask<Result<GetClinicResponse>> IQueryHandler<GetClinicByIdQuery, Result<GetClinicResponse>>.Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
    {
        var clinic = new GetClinicResponse(request.id, "ClinicName", "ClinicAddress");
        return ValueTask.FromResult(Result.Ok(clinic));
    }
}
