namespace ProjectName.Infrastructure.Common.Identity.Options;

/// <summary>
/// Represents the options for configuring downstream API access in the application, including the base URL and required scopes for authentication and authorization.
/// </summary>
public class DownstreamApiOptions
{
    public required string BaseUrl { get; set; }
    public required string[] Scopes { get; set; }
}
