using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Commands;

/// <summary>
/// Represents a command to delete a vaccine from the application.
/// </summary>
/// <param name="Id">The unique identifier of the vaccine to be deleted.</param>
public sealed record DeleteVaccineCommand(Guid Id) : ICommand<Result>;

internal sealed class DeleteVaccineHandler(IVaccineRepository repository) : ICommandHandler<DeleteVaccineCommand, Result>
{
    async ValueTask<Result> ICommandHandler<DeleteVaccineCommand, Result>.Handle(DeleteVaccineCommand request, CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            return Result.Fail(new NotFoundError($"Vaccine '{request.Id}' was not found."));
        }

        return Result.Ok();
    }
}
