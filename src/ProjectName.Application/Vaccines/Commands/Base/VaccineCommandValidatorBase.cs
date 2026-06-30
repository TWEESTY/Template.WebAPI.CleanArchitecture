using FluentValidation;

namespace ProjectName.Application.Vaccines.Commands;

public interface ICreateOrUpdateVaccineCommand
{
    string Code { get; }

    string Name { get; }
}

public abstract class VaccineCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICreateOrUpdateVaccineCommand
{
    protected VaccineCommandValidatorBase()
    {
        _ = RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
