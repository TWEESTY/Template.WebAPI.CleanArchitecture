using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Appointments.Common;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Appointments.Commands;

/// <summary>
/// Represents a command to create an appointment in the application.
/// </summary>
/// <param name="PetId">The unique identifier of the pet for the appointment.</param>
/// <param name="VeterinarianId">The unique identifier of the veterinarian for the appointment.</param>
/// <param name="ClinicId">The unique identifier of the clinic for the appointment.</param>
/// <param name="StartAt">The start time of the appointment in UTC.</param>
/// <param name="EndAt">The end time of the appointment in UTC.</param>
/// <param name="Reason">The reason for the appointment.</param>
public sealed record CreateAppointmentCommand(Guid PetId, Guid VeterinarianId, Guid ClinicId, DateTimeOffset StartAt, DateTimeOffset EndAt, string Reason)
    : ICommand<Result<GetAppointmentResponse>>;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        _ = RuleFor(x => x.PetId)
            .NotEmpty();

        _ = RuleFor(x => x.VeterinarianId)
            .NotEmpty();

        _ = RuleFor(x => x.ClinicId)
            .NotEmpty();

        _ = RuleFor(x => x.StartAt)
            .NotEqual(default(DateTimeOffset));

        _ = RuleFor(x => x.EndAt)
            .NotEqual(default(DateTimeOffset))
            .GreaterThan(x => x.StartAt);

        _ = RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}

internal sealed class CreateAppointmentHandler(IAppointmentRepository repository) : ICommandHandler<CreateAppointmentCommand, Result<GetAppointmentResponse>>
{
    async ValueTask<Result<GetAppointmentResponse>> ICommandHandler<CreateAppointmentCommand, Result<GetAppointmentResponse>>.Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        Appointment appointment = new(request.PetId, request.VeterinarianId, request.ClinicId, request.StartAt, request.EndAt, request.Reason);
        await repository.AddAsync(appointment, cancellationToken);

        return Result.Ok(new GetAppointmentResponse(
            appointment.Id,
            appointment.PetId,
            appointment.VeterinarianId,
            appointment.ClinicId,
            appointment.StartAt,
            appointment.EndAt,
            appointment.Reason,
            appointment.Status.Name));
    }
}
