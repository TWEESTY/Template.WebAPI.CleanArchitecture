using ProjectName.Web.Api.Appointments.Commands;
using ProjectName.Web.Api.Appointments.Queries;

namespace ProjectName.Web.Api.Appointments;

public static class AppointmentsEndpointsGroup
{
    public const string BasePath = "/api/appointments";
    public const string GroupName = "Appointments";

    public static RouteGroupBuilder MapAppointmentsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        group.MapGet(string.Empty, GetAppointmentsEndpoint.HandleAsync);
        group.MapGet("{id:guid}", GetAppointmentByIdEndpoint.HandleAsync);
        group.MapPost(string.Empty, CreateAppointmentEndpoint.HandleAsync);
        group.MapDelete("{id:guid}", DeleteAppointmentEndpoint.HandleAsync);
        group.MapPost("{id:guid}/cancel", CancelAppointmentEndpoint.HandleAsync);
        group.MapPost("{id:guid}/complete", CompleteAppointmentEndpoint.HandleAsync);

        return group;
    }
}
