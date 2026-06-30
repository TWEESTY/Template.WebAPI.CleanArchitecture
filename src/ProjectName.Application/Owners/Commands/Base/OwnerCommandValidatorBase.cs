using FluentValidation;

namespace ProjectName.Application.Owners.Commands;

public interface ICreateOrUpdateOwnerCommand
{
    string FirstName { get; }

    string LastName { get; }

    string Email { get; }

    string PhoneNumber { get; }
}

public abstract class OwnerCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICreateOrUpdateOwnerCommand
{
    protected OwnerCommandValidatorBase()
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

        _ = RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(30);
    }
}
