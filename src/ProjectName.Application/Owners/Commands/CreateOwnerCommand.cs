using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Owners.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Owners.Commands;

/// <summary>
/// Represents a command to create a new owner in the application.
/// </summary>
/// <param name="FirstName">The first name of the owner.</param>
/// <param name="LastName">The last name of the owner.</param>
/// <param name="Email">The email address of the owner.</param>
/// <param name="PhoneNumber">The phone number of the owner.</param>
public sealed record CreateOwnerCommand(string FirstName, string LastName, string Email, string PhoneNumber) : ICommand<Result<GetOwnerResponse>>, ICreateOrUpdateOwnerCommand;

public sealed class CreateOwnerCommandValidator : OwnerCommandValidatorBase<CreateOwnerCommand>
{
    public CreateOwnerCommandValidator()
    {
    }
}

internal sealed class CreateOwnerHandler(IOwnerRepository repository) : ICommandHandler<CreateOwnerCommand, Result<GetOwnerResponse>>
{
    async ValueTask<Result<GetOwnerResponse>> ICommandHandler<CreateOwnerCommand, Result<GetOwnerResponse>>.Handle(CreateOwnerCommand request, CancellationToken cancellationToken)
    {
        Owner owner = new(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        await repository.AddAsync(owner, cancellationToken);

        return Result.Ok(new GetOwnerResponse(owner.Id, owner.FirstName, owner.LastName, owner.Email, owner.PhoneNumber));
    }
}
