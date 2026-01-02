using System;
using FluentResults;

namespace ProjectName.Application.Common.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError() : base()
    {
    }

    public UnauthorizedError(string message) : base(message)
    {
    }
}
