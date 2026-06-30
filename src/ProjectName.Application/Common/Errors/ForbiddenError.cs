using FluentResults;

namespace ProjectName.Application.Common.Errors;

/// <summary>
/// Represents a forbidden error that can be used in the application.
/// </summary>
public class ForbiddenError : Error
{
    public ForbiddenError() : base()
    {
    }

    public ForbiddenError(string message) : base(message)
    {
    }
}
