using FluentResults;

namespace ProjectName.Application.Common.Errors;

/// <summary>
/// Represents an unexpected error that can be used in the application.
/// </summary>
public class UnexpectedError : Error
{
    public UnexpectedError() : base()
    {
    }

    public UnexpectedError(string message) : base(message)
    {
    }
}

