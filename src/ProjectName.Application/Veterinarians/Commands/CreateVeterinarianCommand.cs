using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Commands;

/// <summary>
/// Represents a command to create a new veterinarian in the application.
/// </summary>
/// <param name="FirstName">The first name of the veterinarian.</param>
/// <param name="LastName">The last name of the veterinarian.</param>
/// <param name="Email">The email address of the veterinarian.</param>
/// <param name="LicenseNumber">The license number of the veterinarian.</param>
public sealed record CreateVeterinarianCommand(string FirstName, string LastName, string Email, string LicenseNumber) : ICommand<Result<GetVeterinarianResponse>>, ICreateOrUpdateVeterinarianCommand;

public sealed class CreateVeterinarianCommandValidator : VeterinarianCommandValidatorBase<CreateVeterinarianCommand>
{
    public CreateVeterinarianCommandValidator()
    {
    }
}

internal sealed class CreateVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<CreateVeterinarianCommand, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> ICommandHandler<CreateVeterinarianCommand, Result<GetVeterinarianResponse>>.Handle(CreateVeterinarianCommand request, CancellationToken cancellationToken)
    {
        Veterinarian veterinarian = new(request.FirstName, request.LastName, request.Email, request.LicenseNumber);
        await repository.AddAsync(veterinarian, cancellationToken);

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
