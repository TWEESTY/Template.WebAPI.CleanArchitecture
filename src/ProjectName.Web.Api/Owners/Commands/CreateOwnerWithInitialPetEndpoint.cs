using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Commands;
using ProjectName.Application.Owners.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Owners.Common;

namespace ProjectName.Web.Api.Owners.Commands;

/// <summary>
/// Represents the endpoint for creating a new owner with an initial pet. This endpoint accepts owner and pet details and returns the created owner with pet information or validation errors if the request is invalid.
/// </summary>
internal static class CreateOwnerWithInitialPetEndpoint
{
    internal static async Task<Results<Ok<GetOwnerWithInitialPetCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateOwnerWithInitialPetRequest request)
    {
        Result<GetOwnerWithInitialPetResponse> result = await mediator.Send(new CreateOwnerWithInitialPetCommand(
            request.OwnerFirstName,
            request.OwnerLastName,
            request.OwnerEmail,
            request.OwnerPhoneNumber,
            request.PetName,
            request.PetSpecies,
            request.PetBirthDate));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(GetOwnerWithInitialPetCommonResponseEndpoint.Create(result.Value));
        }

        if (result.HasError<UnauthorizedError>())
        {
            return TypedResults.Unauthorized();
        }

        if (result.HasError<ForbiddenError>())
        {
            return TypedResults.Forbid();
        }

        if (result.HasError<ValidationError>())
        {
            return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());
        }

        return TypedResults.InternalServerError();
    }

    internal sealed record CreateOwnerWithInitialPetRequest(
        string OwnerFirstName,
        string OwnerLastName,
        string OwnerEmail,
        string OwnerPhoneNumber,
        string PetName,
        int PetSpecies,
        DateTimeOffset PetBirthDate);
}