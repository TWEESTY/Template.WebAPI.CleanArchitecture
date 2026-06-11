using ProjectName.Web.Api.Vaccines.Commands;
using ProjectName.Web.Api.Vaccines.Queries;

namespace ProjectName.Web.Api.Vaccines;

public static class VaccinesEndpointsGroup
{
    public const string BasePath = "/api/vaccines";
    public const string GroupName = "Vaccines";

    public static RouteGroupBuilder MapVaccinesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, GetVaccinesEndpoint.HandleAsync);
        group.MapGet("{id:guid}", GetVaccineByIdEndpoint.HandleAsync);
        group.MapPost(string.Empty, CreateVaccineEndpoint.HandleAsync);
        group.MapPut("{id:guid}", UpdateVaccineEndpoint.HandleAsync);
        group.MapDelete("{id:guid}", DeleteVaccineEndpoint.HandleAsync);

        return group;
    }
}
