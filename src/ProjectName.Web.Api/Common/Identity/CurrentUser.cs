using System.Security.Claims;
using ProjectName.Application.Common.Identity;

namespace ProjectName.Web.Api.Common.Identity;

/// <summary>
/// Represents the current user in the web API context, providing access to the user's ID, roles, and permissions based on the HTTP context. This class implements the ICurrentUser interface and retrieves user information from the claims present in the HTTP request.
/// </summary>
/// <param name="httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public IList<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList() ?? [];
    public IList<string> Permissions => _httpContextAccessor.HttpContext?.User?.FindAll("permissions").Select(x => x.Value).ToList() ?? [];
}
