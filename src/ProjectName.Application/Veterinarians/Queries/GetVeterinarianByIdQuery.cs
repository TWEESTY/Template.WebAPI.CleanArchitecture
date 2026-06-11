using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;

namespace ProjectName.Application.Veterinarians.Queries;

public sealed record GetVeterinarianByIdQuery(Guid Id) : IQuery<Result<GetVeterinarianResponse>>;

public sealed class GetVeterinarianByIdHandler(IVeterinarianRepository repository) : IQueryHandler<GetVeterinarianByIdQuery, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> IQueryHandler<GetVeterinarianByIdQuery, Result<GetVeterinarianResponse>>.Handle(GetVeterinarianByIdQuery request, CancellationToken cancellationToken)
    {
        var veterinarian = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (veterinarian is null)
        {
            return Result.Fail(new NotFoundError($"Veterinarian '{request.Id}' was not found."));
        }

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
