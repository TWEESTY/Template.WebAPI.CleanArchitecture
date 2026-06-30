using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Commands;

/// <summary>
/// Represents a command to update an existing vaccine's information in the application.
/// </summary>
/// <param name="Id">The unique identifier of the vaccine to be updated.</param>
/// <param name="Code">The new code of the vaccine.</param>
/// <param name="Name">The new name of the vaccine.</param>
public sealed record UpdateVaccineCommand(Guid Id, string Code, string Name) : ICommand<Result<GetVaccineResponse>>, ICreateOrUpdateVaccineCommand;

public sealed class UpdateVaccineCommandValidator : VaccineCommandValidatorBase<UpdateVaccineCommand>
{
    public UpdateVaccineCommandValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class UpdateVaccineHandler(IVaccineRepository repository) : ICommandHandler<UpdateVaccineCommand, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> ICommandHandler<UpdateVaccineCommand, Result<GetVaccineResponse>>.Handle(UpdateVaccineCommand request, CancellationToken cancellationToken)
    {
        Vaccine? vaccine = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (vaccine is null)
        {
            return Result.Fail(new NotFoundError($"Vaccine '{request.Id}' was not found."));
        }

        vaccine.UpdateDetails(request.Code, request.Name);
        await repository.UpdateAsync(vaccine, cancellationToken);

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
