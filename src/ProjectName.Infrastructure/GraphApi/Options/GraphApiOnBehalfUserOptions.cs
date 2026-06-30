using ProjectName.Infrastructure.Common.Identity.Options;

namespace ProjectName.Infrastructure.GraphApi.Options;

/// <summary>
/// Represents the options for configuring Microsoft Graph API on behalf of a user, including default scopes, tenant ID, client credentials, and base URL.
/// </summary>
public class GraphApiOnBehalfUserOptions : DownstreamApiOptions
{
    public const string ConfigurationSectionName = $"{DownstreamsApiOptions.ConfigurationSectionName}:{nameof(DownstreamsApiOptions.GraphApi)}";
}
