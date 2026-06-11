using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;

namespace ProjectName.Application.Owners.Queries;

public sealed record GetOwnerByIdQuery(Guid Id) : IQuery<Result<GetOwnerResponse>>;

public sealed class GetOwnerByIdHandler(IOwnerRepository repository) : IQueryHandler<GetOwnerByIdQuery, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> IQueryHandler<GetOwnerByIdQuery, Result<GetOwnerResponse>>.Handle(GetOwnerByIdQuery request, CancellationToken cancellationToken)
    {
        var owner = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (owner is null)
        {
            return Result.Fail(new NotFoundError($"Owner '{request.Id}' was not found."));
        }

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
