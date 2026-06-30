using FluentValidation;

namespace ProjectName.Application.Clinics.Commands;

public interface ICreateOrUpdateClinicCommand
{
    string Name { get; }

    string Address { get; }
}

public abstract class ClinicCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICreateOrUpdateClinicCommand
{
    protected ClinicCommandValidatorBase()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        _ = RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);
    }
}
