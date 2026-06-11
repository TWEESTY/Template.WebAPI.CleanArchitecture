using FluentResults;
using Mediator;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Commands;

public sealed record CreateVeterinarianCommand(string FirstName, string LastName, string Email, string LicenseNumber) : ICommand<Result<GetVeterinarianResponse>>;

public sealed class CreateVeterinarianHandler(IVeterinarianRepository repository) : ICommandHandler<CreateVeterinarianCommand, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> ICommandHandler<CreateVeterinarianCommand, Result<GetVeterinarianResponse>>.Handle(CreateVeterinarianCommand request, CancellationToken cancellationToken)
    {
        var veterinarian = new Veterinarian(request.FirstName, request.LastName, request.Email, request.LicenseNumber);
        await repository.AddAsync(veterinarian, cancellationToken);

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
