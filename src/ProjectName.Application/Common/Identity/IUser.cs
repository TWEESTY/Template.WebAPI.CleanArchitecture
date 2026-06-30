namespace ProjectName.Application.Common.Identity;

/// <summary>
/// Represents the current user in the application.
/// </summary>
public interface ICurrentUser
{
    string? Id { get; }
    IList<string> Roles { get; }
    IList<string> Permissions { get; }
}
