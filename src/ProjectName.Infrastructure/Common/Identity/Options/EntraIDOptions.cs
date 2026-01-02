using Microsoft.Identity.Abstractions;

namespace ProjectName.Infrastructure.Common.Identity.Options;

public class EntraIDOptions
{
    public const string ConfigurationSectionName = "EntraID";

    public required string Instance { get; set; } = "https://login.microsoftonline.com/";
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string TenantId { get; set; }
    public required string CallbackPath { get; set; } = "/signin-oidc";
    public required string SignedOutCallbackPath { get; set; } = "/signout-oidc";
    public required string ErrorPath { get; set; } = "/error";
    public required string CustomErrorPath { get; set; } = "https://localhost:4200/login";

    public IEnumerable<CredentialDescription>? ClientCredentials { get; set; }
}
