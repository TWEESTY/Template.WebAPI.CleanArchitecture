namespace ProjectName.Application.Common.Identity;

public interface ICurrentUser
{
    string? Id { get; }
    IList<string> Roles { get; }
    IList<string> Permissions { get; }
}