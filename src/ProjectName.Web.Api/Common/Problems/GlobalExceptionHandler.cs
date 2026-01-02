using Microsoft.AspNetCore.Diagnostics;

namespace ProjectName.Web.Api.Common.Problems;

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