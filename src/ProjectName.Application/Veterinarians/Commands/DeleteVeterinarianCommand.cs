using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Commands;

/// <summary>
/// Represents a command to delete a veterinarian from the application.
/// </summary>
/// <param name="Id">The unique identifier of the veterinarian to delete.</param>
public sealed record DeleteVeterinarianCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<DeleteVeterinarianCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeleteVeterinarianCommand, Result>.Handle(DeleteVeterinarianCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Veterinarian '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
