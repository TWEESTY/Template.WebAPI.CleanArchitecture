using FluentResults;
using Mediator;

namespace ProjectName.Application.Pets.Commands;

public sealed record DeletePetCommand(Guid Id) : ICommand<Result>;

public sealed class DeletePetHandler : ICommandHandler<DeletePetCommand, Result>
{
    ValueTask<Result> ICommandHandler<DeletePetCommand, Result>.Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Result.Ok());
    }
}