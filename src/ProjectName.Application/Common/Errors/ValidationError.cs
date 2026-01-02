using FluentResults;

namespace ProjectName.Application.Common.Errors;

public class ValidationError(string identifier, string message) : Error(message)
{
    public string Identifier { get; set; } = identifier;
}
