using FluentResults;
using Mediator;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Queries;

/// <summary>
/// Represents a query to retrieve a veterinarian by its unique identifier in the application.
/// </summary>
/// <param name="Id">The unique identifier of the veterinarian to retrieve.</param>
public sealed record GetVeterinarianByIdQuery(Guid Id) : IQuery<Result<GetVeterinarianResponse>>;

internal sealed class GetVeterinarianByIdHandler(IVeterinarianRepository repository) : IQueryHandler<GetVeterinarianByIdQuery, Result<GetVeterinarianResponse>>
{
    async ValueTask<Result<GetVeterinarianResponse>> IQueryHandler<GetVeterinarianByIdQuery, Result<GetVeterinarianResponse>>.Handle(GetVeterinarianByIdQuery request, CancellationToken cancellationToken)
    {
        Veterinarian? veterinarian = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (veterinarian is null)
        {
            return Result.Fail(new NotFoundError($"Veterinarian '{request.Id}' was not found."));
        }

        return Result.Ok(new GetVeterinarianResponse(veterinarian.Id, veterinarian.FirstName, veterinarian.LastName, veterinarian.Email, veterinarian.LicenseNumber));
    }
}
