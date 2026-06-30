using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Commands;

/// <summary>
/// Represents a command to create a new vaccine in the application.
/// </summary>
/// <param name="Code">The code of the vaccine.</param>
/// <param name="Name">The name of the vaccine.</param>
public sealed record CreateVaccineCommand(string Code, string Name) : ICommand<Result<GetVaccineResponse>>, ICreateOrUpdateVaccineCommand;

public sealed class CreateVaccineCommandValidator : VaccineCommandValidatorBase<CreateVaccineCommand>
{
    public CreateVaccineCommandValidator()
    {
    }
}

internal sealed class CreateVaccineHandler(IVaccineRepository repository) : ICommandHandler<CreateVaccineCommand, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> ICommandHandler<CreateVaccineCommand, Result<GetVaccineResponse>>.Handle(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        Vaccine vaccine = new(request.Code, request.Name);
        await repository.AddAsync(vaccine, cancellationToken);

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
