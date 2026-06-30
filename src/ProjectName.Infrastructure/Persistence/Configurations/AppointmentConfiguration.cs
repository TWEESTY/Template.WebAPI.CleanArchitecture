using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Appointment entity, specifying table name, primary key, required properties, and property conversions for the database context.
/// </summary>
internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        _ = builder.ToTable("Appointments");

        _ = builder.HasKey(a => a.Id);

        _ = builder.Property(a => a.PetId)
            .IsRequired();

        _ = builder.Property(a => a.VeterinarianId)
            .IsRequired();

        _ = builder.Property(a => a.ClinicId)
            .IsRequired();

        _ = builder.Property(a => a.StartAt)
            .HasConversion(
                v => v.UtcDateTime,
                v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)))
            .IsRequired();

        _ = builder.Property(a => a.EndAt)
            .HasConversion(
                v => v.UtcDateTime,
                v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)))
            .IsRequired();

        _ = builder.Property(a => a.Reason)
            .IsRequired()
            .HasMaxLength(500);

        _ = builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion(
                s => s.Value,
                v => AppointmentStatus.FromValue(v));
    }
}
