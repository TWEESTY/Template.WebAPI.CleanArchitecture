using ProjectName.Infrastructure.Common.Identity.Options;

namespace ProjectName.Infrastructure.GraphApi.Options;

public class GraphApiOnBehalfUserOptions : DownstreamApiOptions
{
    public const string ConfigurationSectionName = $"{DownstreamsApiOptions.ConfigurationSectionName}:{nameof(DownstreamsApiOptions.GraphApi)}";
}
