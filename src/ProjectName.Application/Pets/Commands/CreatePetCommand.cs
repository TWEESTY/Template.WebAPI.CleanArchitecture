using FluentResults;
using Mediator;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record CreatePetCommand(string Name, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>;

public sealed class CreatePetHandler : ICommandHandler<CreatePetCommand, Result<GetPetResponse>>
{
    ValueTask<Result<GetPetResponse>> ICommandHandler<CreatePetCommand, Result<GetPetResponse>>.Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        var response = new GetPetResponse(Guid.NewGuid(), request.Name, request.BirthDate);
        return ValueTask.FromResult(Result.Ok(response));
    }
}