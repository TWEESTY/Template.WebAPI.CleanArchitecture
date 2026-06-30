using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Commands;

/// <summary>
/// Represents a command to update an existing veterinarian's information in the application.
/// </summary>
/// <param name="Id">The unique identifier of the veterinarian to update.</param>
/// <param name="FirstName">The new first name of the veterinarian.</param>
/// <param name="LastName">The new last name of the veterinarian.</param>
/// <param name="Email">The new email address of the veterinarian.</param>
/// <param name="LicenseNumber">The new license number of the veterinarian.</param>
public sealed record UpdateVeterinarianCommand(Guid Id, string FirstName, string LastName, string Email, string LicenseNumber) : ICommand<Result<GetVeterinarianResponse>>, ICreateOrUpdateVeterinarianCommand;

public sealed class UpdateVeterinarianCommandValidator : VeterinarianCommandValidatorBase<UpdateVeterinarianCommand>
{
    public UpdateVeterinarianCommandValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class UpdateVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<UpdateVeterinarianCommand, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> ICommandHandler<UpdateVeterinarianCommand, Result<GetVeterinarianResponse>>.Handle(UpdateVeterinarianCommand request, CancellationToken cancellationToken)
    {
        Veterinarian? veterinarian = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (veterinarian is null)
        {
            return Result.Fail(new NotFoundError($"Veterinarian '{request.Id}' was not found."));
        }

        veterinarian.UpdateProfile(request.FirstName, request.LastName, request.Email, request.LicenseNumber);
        await repository.UpdateAsync(veterinarian, cancellationToken);

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
