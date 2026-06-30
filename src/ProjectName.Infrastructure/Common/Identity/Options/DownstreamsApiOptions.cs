using ProjectName.Infrastructure.GraphApi.Options;

namespace ProjectName.Infrastructure.Common.Identity.Options;

/// <summary>
/// Represents the options for configuring downstream API access in the application, including the Graph API options and initial scopes required for authentication and authorization.
/// </summary>
public class DownstreamsApiOptions
{
    public const string ConfigurationSectionName = "DownstreamsApi";

    public required GraphApiOnBehalfUserOptions GraphApi { get; set; }
    public required string[] InitialScopes { get; set; }
}
