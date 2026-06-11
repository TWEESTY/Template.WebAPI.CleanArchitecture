using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;

namespace ProjectName.Application.Owners.Commands;

public sealed record DeleteOwnerCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteOwnerHandler(IOwnerRepository repository) : ICommandHandler<DeleteOwnerCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeleteOwnerCommand, Result>.Handle(DeleteOwnerCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Owner '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
