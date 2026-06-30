using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace ProjectName.Infrastructure.Common.Http;

/// <summary>
/// A custom HTTP message handler that logs Refit requests and responses, including method, URI, query parameters, headers, and body content. Sensitive information such as authorization headers and cookies are redacted from the logs.
/// </summary>
/// <param name="logger">The logger instance used to log request and response information.</param>
internal sealed class RefitRequestLoggingHandler(ILogger<RefitRequestLoggingHandler> logger) : DelegatingHandler
{
    private const int _maxLoggedBodyLength = 4000;
    private readonly ILogger<RefitRequestLoggingHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            string requestBody = await ReadRequestBodyAsync(request, cancellationToken);
            Dictionary<string, string> requestHeaders = BuildHeaders(request.Headers);

            _logger.LogInformation(
                "Sending Refit request {Method} {Uri}. Query={Query}. Headers={Headers}. Body={Body}",
                request.Method.Method,
                request.RequestUri,
                request.RequestUri?.Query,
                requestHeaders,
                requestBody);
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        stopwatch.Stop();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "Received Refit response {StatusCode} for {Method} {Uri} in {ElapsedMs}ms",
                (int)response.StatusCode,
                request.Method.Method,
                request.RequestUri,
                stopwatch.ElapsedMilliseconds);
        }

        return response;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return string.Empty;
        }

        string body = await request.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        if (body.Length <= _maxLoggedBodyLength)
        {
            return body;
        }

        return $"{body[.._maxLoggedBodyLength]}... [truncated]";
    }

    private static Dictionary<string, string> BuildHeaders(HttpRequestHeaders headers)
    {
        Dictionary<string, string> normalizedHeaders = new(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
        {
            if (string.Equals(header.Key, "Authorization", StringComparison.OrdinalIgnoreCase)
                || string.Equals(header.Key, "Cookie", StringComparison.OrdinalIgnoreCase)
                || string.Equals(header.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase))
            {
                normalizedHeaders[header.Key] = "[REDACTED]";
                continue;
            }

            normalizedHeaders[header.Key] = string.Join(",", header.Value);
        }

        return normalizedHeaders;
    }
}
