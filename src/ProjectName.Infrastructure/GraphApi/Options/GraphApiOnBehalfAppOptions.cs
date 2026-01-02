namespace ProjectName.Infrastructure.GraphApi.Options;

public class GraphApiOnBehalfAppOptions
{
    public const string ConfigurationNodeName = "GraphApiOnBehalfApp";

    public required string[] DefaultScopes { get; set; } = ["https://graph.microsoft.com/.default"];
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string BaseUrl { get; set; } = "https://graph.microsoft.com/v1.0";
}
