using System.Security.Claims;
using ProjectName.Application.Common.Identity;

namespace ProjectName.Web.Api.Common.Identity;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{   
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public IList<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList() ?? new List<string>();
    public IList<string> Permissions => _httpContextAccessor.HttpContext?.User?.FindAll("permissions").Select(x => x.Value).ToList() ?? new List<string>();
}
