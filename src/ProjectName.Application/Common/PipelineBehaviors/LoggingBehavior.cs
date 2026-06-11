using Mediator;
using Microsoft.Extensions.Logging;

namespace ProjectName.Application.Common.PipelineBehaviors;

public sealed class LoggingBehavior<TMessage, TResponse>
(ILogger<LoggingBehavior<TMessage, TResponse>> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<LoggingBehavior<TMessage, TResponse>> _logger = logger;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        string messageName = typeof(TMessage).Name;

        // Check if request type has ExcludeFromRequestLogging attribute
        bool excludeFromDetailedLogging = typeof(TMessage)
            .GetCustomAttributes(typeof(ExcludeMessageFromLoggingAttribute), false)
            .Length > 0;

        // Log start - only include request details if not excluded
        if (excludeFromDetailedLogging)
        {
            _logger.LogInformation("Starting request: {MessageName} (details excluded due to large payload)", messageName);
        }
        else
        {
            _logger.LogInformation("Starting request: {MessageName} - {@Message}", messageName, message);
        }

        var response = await next(message, cancellationToken);

        // Log completion - only include request details if not excluded
        if (excludeFromDetailedLogging)
        {
            _logger.LogInformation("Completed request: {MessageName} (details excluded due to large payload)", messageName);
        }
        else
        {
            _logger.LogInformation("Completed request: {MessageName} - {@Message}", messageName, message);
        }

        return response;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExcludeMessageFromLoggingAttribute : Attribute
    {
    }
}
