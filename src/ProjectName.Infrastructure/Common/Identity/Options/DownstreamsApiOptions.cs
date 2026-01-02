using ProjectName.Infrastructure.GraphApi.Options;

namespace ProjectName.Infrastructure.Common.Identity.Options;

public class DownstreamsApiOptions
{
    public const string ConfigurationSectionName = "DownstreamsApi";

    public required GraphApiOnBehalfUserOptions GraphApi { get; set; }
    public required string[] InitialScopes { get; set; }
}
