namespace ProjectName.Application.Common.Identity;

public interface IUser
{
    string? Id { get; }
    IList<string> Roles { get; }
    IList<string> Permissions { get; }
}