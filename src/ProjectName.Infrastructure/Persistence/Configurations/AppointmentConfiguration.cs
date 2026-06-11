using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.PetId)
            .IsRequired();

        builder.Property(a => a.VeterinarianId)
            .IsRequired();

        builder.Property(a => a.ClinicId)
            .IsRequired();

        builder.Property(a => a.StartAtUtc)
            .IsRequired();

        builder.Property(a => a.EndAtUtc)
            .IsRequired();

        builder.Property(a => a.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion(
                s => s.Value,
                v => AppointmentStatus.FromValue(v));
    }
}
