using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record UpdatePetCommand(Guid Id, string Name, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>;

public sealed class UpdatePetHandler(IPetRepository petRepository) : ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>
{
    async ValueTask<Result<GetPetResponse>> ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>.Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pet is null)
        {
            return Result.Fail(new NotFoundError($"Pet '{request.Id}' was not found."));
        }

        pet.Rename(request.Name);
        pet.ChangeBirthDate(DateOnly.FromDateTime(request.BirthDate.UtcDateTime));
        await petRepository.UpdateAsync(pet, cancellationToken);

        var response = new GetPetResponse(
            pet.Id,
            pet.Name,
            new DateTimeOffset(pet.BirthDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));

        return Result.Ok(response);
    }
}