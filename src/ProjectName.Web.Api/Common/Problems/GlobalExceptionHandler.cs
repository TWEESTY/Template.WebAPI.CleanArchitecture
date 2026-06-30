using Microsoft.AspNetCore.Diagnostics;

namespace ProjectName.Web.Api.Common.Problems;

/// <summary>
/// Represents a global exception handler for the web API, responsible for logging unhandled exceptions that occur during request processing. This class implements the IExceptionHandler interface and provides a mechanism to capture and log exceptions, allowing for centralized error handling and improved observability of application issues.
/// </summary>
/// <param name="logger"></param>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception has occurred while executing the request");

        // Continue with the default behavior
        return ValueTask.FromResult(false);
    }
}
