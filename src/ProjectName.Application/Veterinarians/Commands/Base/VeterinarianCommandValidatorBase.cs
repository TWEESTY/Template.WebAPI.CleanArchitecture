using FluentValidation;

namespace ProjectName.Application.Veterinarians.Commands;

public interface ICreateOrUpdateVeterinarianCommand
{
    string FirstName { get; }

    string LastName { get; }

    string Email { get; }

    string LicenseNumber { get; }
}

public abstract class VeterinarianCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICreateOrUpdateVeterinarianCommand
{
    protected VeterinarianCommandValidatorBase()
    {
        _ = RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        _ = RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .MaximumLength(50);
    }
}
