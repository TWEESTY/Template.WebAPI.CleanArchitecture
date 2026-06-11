using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Common.Exceptions;

namespace ProjectName.Application.Common.PipelineBehaviors;

/// <summary>
/// Pipeline behavior to catch unhandled exceptions and log them, then return a generic error response.
/// </summary>
/// <typeparam name="TMessage">The type of the message being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
/// <param name="logger">The logger instance.</param>
public sealed class UnhandledExceptionBehavior<TMessage, TResponse>
(ILogger<UnhandledExceptionBehavior<TMessage, TResponse>> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : ResultBase, new()
{
    private readonly ILogger<UnhandledExceptionBehavior<TMessage, TResponse>> _logger = logger;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (DomainException ex)
        {
            var requestName = typeof(TMessage).Name;
            _logger.LogWarning(ex, "Domain Exception for Message {Name} {@Request}", requestName, message);

            var result = new TResponse();
            string propertyName = ex.PropertyName ?? nameof(TMessage);
            result.Reasons.Add(new ValidationError(propertyName, ex.Message));
            return result;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TMessage).Name;
            _logger.LogError(ex, "Unhandled Exception for Message {Name} {@Request}", requestName, message);

            var result = new TResponse();
            result.Reasons.Add(new UnexpectedError());
            return result;
        }
    }
}
