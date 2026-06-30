using FluentResults;

namespace ProjectName.Application.Common.Errors;

/// <summary>
/// Represents a not found error that can be used in the application.
/// </summary>
public class NotFoundError : Error
{
    public NotFoundError() : base()
    {
    }

    public NotFoundError(string message) : base(message)
    {
    }
}
