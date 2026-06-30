using FluentResults;
using Mediator;
using ProjectName.Application.Common.PipelineBehaviors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Pets.Commands;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Owners.Commands;

/// <summary>
/// Represents a command to create a new owner along with their initial pet in the application.
/// </summary>
/// <param name="OwnerFirstName">The first name of the owner.</param>
/// <param name="OwnerLastName">The last name of the owner.</param>
/// <param name="OwnerEmail">The email address of the owner.</param>
/// <param name="OwnerPhoneNumber">The phone number of the owner.</param>
/// <param name="PetName">The name of the pet.</param>
/// <param name="PetSpecies">The species of the pet.</param>
/// <param name="PetBirthDate">The birth date of the pet.</param>
[UseUnitOfWork]
public sealed record CreateOwnerWithInitialPetCommand(
    string OwnerFirstName,
    string OwnerLastName,
    string OwnerEmail,
    string OwnerPhoneNumber,
    string PetName,
    int PetSpecies,
    DateTimeOffset PetBirthDate) : ICommand<Result<GetOwnerWithInitialPetResponse>>;

internal sealed class CreateOwnerWithInitialPetHandler(IMediator mediator) : ICommandHandler<CreateOwnerWithInitialPetCommand, Result<GetOwnerWithInitialPetResponse>>
{
    async ValueTask<Result<GetOwnerWithInitialPetResponse>> ICommandHandler<CreateOwnerWithInitialPetCommand, Result<GetOwnerWithInitialPetResponse>>.Handle(CreateOwnerWithInitialPetCommand request, CancellationToken cancellationToken)
    {
        // This orchestration intentionally chains two commands to demonstrate shared UnitOfWork behavior.
        // In production, you could also implement the same business operation in a single command handler.
        Result<GetOwnerResponse> ownerResult = await mediator.Send(
            new CreateOwnerCommand(request.OwnerFirstName, request.OwnerLastName, request.OwnerEmail, request.OwnerPhoneNumber),
            cancellationToken);

        if (ownerResult.IsFailed)
        {
            return Result.Fail(ownerResult.Errors);
        }

        Result<GetPetResponse> petResult = await mediator.Send(
            new CreatePetCommand(ownerResult.Value.Id, request.PetName, request.PetSpecies, request.PetBirthDate),
            cancellationToken);

        if (petResult.IsFailed)
        {
            return Result.Fail(petResult.Errors);
        }

        return Result.Ok(new GetOwnerWithInitialPetResponse(
            ownerResult.Value.Id,
            ownerResult.Value.FirstName,
            ownerResult.Value.LastName,
            petResult.Value.Id,
            petResult.Value.Name,
            petResult.Value.BirthDate));
    }
}