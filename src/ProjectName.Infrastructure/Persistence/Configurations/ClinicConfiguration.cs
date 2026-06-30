using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Clinic entity, specifying table name, primary key, and required properties for the database context.
/// </summary>
internal sealed class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        _ = builder.ToTable("Clinics");

        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(200);
    }
}
