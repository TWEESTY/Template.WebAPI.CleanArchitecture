using Ardalis.SmartEnum;

namespace ProjectName.Domain.Enums;

public sealed class AppointmentStatus : SmartEnum<AppointmentStatus>
{
    public static readonly AppointmentStatus Scheduled =
        new(nameof(Scheduled), 1);
    public static readonly AppointmentStatus Completed =
        new(nameof(Completed), 2);
    public static readonly AppointmentStatus Cancelled =
        new(nameof(Cancelled), 3);

    private AppointmentStatus(
        string name,
        int value)
        : base(name, value)
    {
    }

    public static IReadOnlyCollection<AppointmentStatus> All =>
    [
        Scheduled,
        Completed,
        Cancelled
    ];
}
