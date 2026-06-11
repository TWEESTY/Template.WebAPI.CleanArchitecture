using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;

namespace ProjectName.Application.Owners.Commands;

public sealed record UpdateOwnerCommand(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber) : ICommand<Result<GetOwnerResponse>>;

public sealed class UpdateOwnerHandler(IOwnerRepository repository) : ICommandHandler<UpdateOwnerCommand, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> ICommandHandler<UpdateOwnerCommand, Result<GetOwnerResponse>>.Handle(UpdateOwnerCommand request, CancellationToken cancellationToken)
    {
        var owner = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (owner is null)
        {
            return Result.Fail(new NotFoundError($"Owner '{request.Id}' was not found."));
        }

        owner.UpdateContactInformation(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        await repository.UpdateAsync(owner, cancellationToken);

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
