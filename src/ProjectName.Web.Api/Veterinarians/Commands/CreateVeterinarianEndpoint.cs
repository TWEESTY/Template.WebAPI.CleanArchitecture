using FluentResults;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Web.Api.Common.Problems;
using ProjectName.Web.Api.Veterinarians.Common;

namespace ProjectName.Web.Api.Veterinarians.Commands;

public static class CreateVeterinarianEndpoint
{
    public static async Task<Results<Ok<GetVeterinarianCommonResponseEndpoint>, UnauthorizedHttpResult, ForbidHttpResult, ValidationProblem, InternalServerError>> HandleAsync(
        IMediator mediator,
        CreateVeterinarianRequest request)
    {
        Result<GetVeterinarianResponse> result = await mediator.Send(new CreateVeterinarianCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.LicenseNumber));

        if (result.IsSuccess) return TypedResults.Ok(GetVeterinarianCommonResponseEndpoint.Create(result.Value));
        if (result.HasError<UnauthorizedError>()) return TypedResults.Unauthorized();
        if (result.HasError<ForbiddenError>()) return TypedResults.Forbid();
        if (result.HasError<ValidationError>()) return TypedResults.ValidationProblem(result.Errors.ToProblemErrors());

        return TypedResults.InternalServerError();
    }

    public sealed record CreateVeterinarianRequest(string FirstName, string LastName, string Email, string LicenseNumber);
}
