using FluentResults;

namespace ProjectName.Application.Common.Errors;

/// <summary>
/// Represents a validation error that can be used in the application.
/// </summary>
/// <param name="identifier">The identifier of the validation error.</param>
/// <param name="message">The message describing the validation error.</param>
public class ValidationError(string identifier, string message) : Error(message)
{
    public string Identifier { get; set; } = identifier;
}
