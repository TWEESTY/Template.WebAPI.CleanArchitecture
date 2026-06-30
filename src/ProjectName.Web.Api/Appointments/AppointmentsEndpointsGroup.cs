using ProjectName.Web.Api.Appointments.Commands;
using ProjectName.Web.Api.Appointments.Queries;

namespace ProjectName.Web.Api.Appointments;

/// <summary>
/// Represents a static class that defines the endpoints for managing appointments in the application. This class provides methods to map the various appointment-related endpoints to their corresponding handlers, allowing clients to perform operations such as retrieving, creating, canceling, completing, and deleting appointments.
/// </summary>
public static class AppointmentsEndpointsGroup
{
    public const string BasePath = "/api/appointments";
    public const string GroupName = "Appointments";

    public static RouteGroupBuilder MapAppointmentsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(BasePath)
            .WithTags(GroupName)
            .RequireAuthorization();

        _ = group.MapGet(string.Empty, GetAppointmentsEndpoint.HandleAsync);
        _ = group.MapGet("{id:guid}", GetAppointmentByIdEndpoint.HandleAsync);
        _ = group.MapPost(string.Empty, CreateAppointmentEndpoint.HandleAsync);
        _ = group.MapDelete("{id:guid}", DeleteAppointmentEndpoint.HandleAsync);
        _ = group.MapPost("{id:guid}/cancel", CancelAppointmentEndpoint.HandleAsync);
        _ = group.MapPost("{id:guid}/complete", CompleteAppointmentEndpoint.HandleAsync);

        return group;
    }
}
