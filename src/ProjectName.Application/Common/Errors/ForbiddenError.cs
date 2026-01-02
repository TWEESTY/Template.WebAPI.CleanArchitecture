using FluentResults;

namespace ProjectName.Application.Common.Errors;

public class ForbiddenError : Error
{
    public ForbiddenError() : base()
    {
    }

    public ForbiddenError(string message) : base(message)
    {
    }
}
