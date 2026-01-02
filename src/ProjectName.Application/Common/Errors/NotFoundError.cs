using System;
using FluentResults;

namespace ProjectName.Application.Common.Errors;

public class NotFoundError : Error
{
    public NotFoundError() : base()
    {
    }

    public NotFoundError(string message) : base(message)
    {
    }
}