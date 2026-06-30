using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Owners.Commands;

/// <summary>
/// Represents a command to update an existing owner's contact information in the application.
/// </summary>
/// <param name="Id">The unique identifier of the owner to be updated.</param>
/// <param name="FirstName">The new first name of the owner.</param>
/// <param name="LastName">The new last name of the owner.</param>
/// <param name="Email">The new email address of the owner.</param>
/// <param name="PhoneNumber">The new phone number of the owner.</param>
public sealed record UpdateOwnerCommand(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber) : ICommand<Result<GetOwnerResponse>>, ICreateOrUpdateOwnerCommand;

public sealed class UpdateOwnerCommandValidator : OwnerCommandValidatorBase<UpdateOwnerCommand>
{
    public UpdateOwnerCommandValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class UpdateOwnerHandler(IOwnerRepository repository) : ICommandHandler<UpdateOwnerCommand, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> ICommandHandler<UpdateOwnerCommand, Result<GetOwnerResponse>>.Handle(UpdateOwnerCommand request, CancellationToken cancellationToken)
    {
        Owner? owner = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (owner is null)
        {
            return Result.Fail(new NotFoundError($"Owner '{request.Id}' was not found."));
        }

        owner.UpdateContactInformation(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        await repository.UpdateAsync(owner, cancellationToken);

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
