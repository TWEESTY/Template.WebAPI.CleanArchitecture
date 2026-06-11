using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Commands;

public sealed record DeleteVaccineCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteVaccineHandler(IVaccineRepository repository) : ICommandHandler<DeleteVaccineCommand, Result>
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
