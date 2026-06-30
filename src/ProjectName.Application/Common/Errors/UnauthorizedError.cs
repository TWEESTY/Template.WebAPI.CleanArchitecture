using FluentResults;

namespace ProjectName.Application.Common.Errors;

/// <summary>
/// Represents an unauthorized error that can be used in the application.
/// </summary>
public class UnauthorizedError : Error
{
    public UnauthorizedError() : base()
    {
    }

    public UnauthorizedError(string message) : base(message)
    {
    }
}
