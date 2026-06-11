using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;

namespace ProjectName.Application.Vaccines.Commands;

public sealed record UpdateVaccineCommand(Guid Id, string Code, string Name) : ICommand<Result<GetVaccineResponse>>;

public sealed class UpdateVaccineHandler(IVaccineRepository repository) : ICommandHandler<UpdateVaccineCommand, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> ICommandHandler<UpdateVaccineCommand, Result<GetVaccineResponse>>.Handle(UpdateVaccineCommand request, CancellationToken cancellationToken)
    {
        var vaccine = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (vaccine is null)
        {
            return Result.Fail(new NotFoundError($"Vaccine '{request.Id}' was not found."));
        }

        vaccine.UpdateDetails(request.Code, request.Name);
        await repository.UpdateAsync(vaccine, cancellationToken);

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
