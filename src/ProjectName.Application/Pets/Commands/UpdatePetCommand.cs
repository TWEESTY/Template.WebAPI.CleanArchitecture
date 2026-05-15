using FluentResults;
using Mediator;
using ProjectName.Application.Pets.Common;

namespace ProjectName.Application.Pets.Commands;

public sealed record UpdatePetCommand(Guid Id, string Name, DateTimeOffset BirthDate) : ICommand<Result<GetPetResponse>>;

public sealed class UpdatePetHandler : ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>
{
    ValueTask<Result<GetPetResponse>> ICommandHandler<UpdatePetCommand, Result<GetPetResponse>>.Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        var response = new GetPetResponse(request.Id, request.Name, request.BirthDate);
        return ValueTask.FromResult(Result.Ok(response));
    }
}