using FluentValidation;

namespace ProjectName.Application.Pets.Commands;

public interface ICreateOrUpdatePetCommand
{
    string Name { get; }

    DateTimeOffset BirthDate { get; }
}

public abstract class PetCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICreateOrUpdatePetCommand
{
    protected PetCommandValidatorBase()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.BirthDate)
            .NotEqual(default(DateTimeOffset));
    }
}
