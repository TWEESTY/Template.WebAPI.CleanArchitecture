using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

/// <summary>
/// Represents a command to delete an existing pet in the application.
/// </summary>
/// <param name="Id">The unique identifier of the pet to be deleted.</param>
public sealed record DeletePetCommand(Guid Id) : ICommand<Result>;

internal sealed class DeletePetHandler(IPetRepository petRepository) : ICommandHandler<DeletePetCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeletePetCommand, Result>.Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await petRepository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
