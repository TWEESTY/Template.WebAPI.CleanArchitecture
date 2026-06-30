using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Owners.Queries;

/// <summary>
/// Represents a query to retrieve an owner by their unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the owner to be retrieved.</param>
public sealed record GetOwnerByIdQuery(Guid Id) : IQuery<Result<GetOwnerResponse>>;

internal sealed class GetOwnerByIdHandler(IOwnerRepository repository) : IQueryHandler<GetOwnerByIdQuery, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> IQueryHandler<GetOwnerByIdQuery, Result<GetOwnerResponse>>.Handle(GetOwnerByIdQuery request, CancellationToken cancellationToken)
    {
        Owner? owner = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (owner is null)
        {
            return Result.Fail(new NotFoundError($"Owner '{request.Id}' was not found."));
        }

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
