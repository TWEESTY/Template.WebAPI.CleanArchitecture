using ProjectName.Web.Api.Owners.Commands;
using ProjectName.Web.Api.Owners.Queries;

namespace ProjectName.Web.Api.Owners;

public static class OwnersEndpointsGroup
{
    public const string BasePath = "/api/owners";
    public const string GroupName = "Owners";

    public static RouteGroupBuilder MapOwnersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, GetOwnersEndpoint.HandleAsync);
        group.MapGet("{id:guid}", GetOwnerByIdEndpoint.HandleAsync);
        group.MapPost(string.Empty, CreateOwnerEndpoint.HandleAsync);
        group.MapPut("{id:guid}", UpdateOwnerEndpoint.HandleAsync);
        group.MapDelete("{id:guid}", DeleteOwnerEndpoint.HandleAsync);

        return group;
    }
}
