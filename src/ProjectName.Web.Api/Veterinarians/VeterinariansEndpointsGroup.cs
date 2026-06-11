using ProjectName.Web.Api.Veterinarians.Commands;
using ProjectName.Web.Api.Veterinarians.Queries;

namespace ProjectName.Web.Api.Veterinarians;

public static class VeterinariansEndpointsGroup
{
    public const string BasePath = "/api/veterinarians";
    public const string GroupName = "Veterinarians";

    public static RouteGroupBuilder MapVeterinariansEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, GetVeterinariansEndpoint.HandleAsync);
        group.MapGet("{id:guid}", GetVeterinarianByIdEndpoint.HandleAsync);
        group.MapPost(string.Empty, CreateVeterinarianEndpoint.HandleAsync);
        group.MapPut("{id:guid}", UpdateVeterinarianEndpoint.HandleAsync);
        group.MapDelete("{id:guid}", DeleteVeterinarianEndpoint.HandleAsync);

        return group;
    }
}
