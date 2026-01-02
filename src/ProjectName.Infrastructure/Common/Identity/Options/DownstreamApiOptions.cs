using ProjectName.Infrastructure.GraphApi.Options;

namespace ProjectName.Infrastructure.Common.Identity.Options;

public class DownstreamApiOptions
{
    public required string BaseUrl { get; set; }
    public required string[] Scopes { get; set; }
}