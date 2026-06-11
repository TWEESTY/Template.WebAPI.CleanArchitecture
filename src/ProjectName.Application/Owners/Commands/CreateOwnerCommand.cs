using FluentResults;
using Mediator;
using ProjectName.Application.Owners.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Owners.Commands;

public sealed record CreateOwnerCommand(string FirstName, string LastName, string Email, string PhoneNumber) : ICommand<Result<GetOwnerResponse>>;

public sealed class CreateOwnerHandler(IOwnerRepository repository) : ICommandHandler<CreateOwnerCommand, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> ICommandHandler<CreateOwnerCommand, Result<GetOwnerResponse>>.Handle(CreateOwnerCommand request, CancellationToken cancellationToken)
    {
        var owner = new Owner(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        await repository.AddAsync(owner, cancellationToken);

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
