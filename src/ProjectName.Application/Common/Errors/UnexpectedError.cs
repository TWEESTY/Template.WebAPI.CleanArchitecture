using System;
using FluentResults;

namespace ProjectName.Application.Common.Errors;

public class UnexpectedError : Error
{
    public UnexpectedError() : base()
    {
    }

    public UnexpectedError(string message) : base(message)
    {
    }
}

