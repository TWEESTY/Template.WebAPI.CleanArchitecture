using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Commands;

public sealed record DeleteVeterinarianCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<DeleteVeterinarianCommand, Result>
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
