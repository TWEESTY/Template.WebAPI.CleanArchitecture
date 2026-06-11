using FluentResults;
using Mediator;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Commands;

public sealed record CreateVaccineCommand(string Code, string Name) : ICommand<Result<GetVaccineResponse>>;

public sealed class CreateVaccineHandler(IVaccineRepository repository) : ICommandHandler<CreateVaccineCommand, Result<GetVaccineResponse>>
{
    async ValueTask<Result<GetVaccineResponse>> ICommandHandler<CreateVaccineCommand, Result<GetVaccineResponse>>.Handle(CreateVaccineCommand request, CancellationToken cancellationToken)
    {
        var vaccine = new Vaccine(request.Code, request.Name);
        await repository.AddAsync(vaccine, cancellationToken);

        return Result.Ok(new GetVaccineResponse(vaccine.Id, vaccine.Code, vaccine.Name));
    }
}
