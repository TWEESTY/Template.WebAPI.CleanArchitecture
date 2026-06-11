using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Commands;

public sealed record UpdateVeterinarianCommand(Guid Id, string FirstName, string LastName, string Email, string LicenseNumber) : ICommand<Result<GetVeterinarianResponse>>;

public sealed class UpdateVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<UpdateVeterinarianCommand, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> ICommandHandler<UpdateVeterinarianCommand, Result<GetVeterinarianResponse>>.Handle(UpdateVeterinarianCommand request, CancellationToken cancellationToken)
    {
        var veterinarian = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (veterinarian is null)
        {
            return Result.Fail(new NotFoundError($"Veterinarian '{request.Id}' was not found."));
        }

        veterinarian.UpdateProfile(request.FirstName, request.LastName, request.Email, request.LicenseNumber);
        await repository.UpdateAsync(veterinarian, cancellationToken);

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
