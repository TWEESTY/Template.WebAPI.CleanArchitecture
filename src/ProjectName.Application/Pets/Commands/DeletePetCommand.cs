using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record DeletePetCommand(Guid Id) : ICommand<Result>;

public sealed class DeletePetHandler(IPetRepository petRepository) : ICommandHandler<DeletePetCommand, Result>
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