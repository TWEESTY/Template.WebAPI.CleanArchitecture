using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the ClinicVeterinarian entity, specifying table name and composite primary key for the database context.
/// </summary>
internal sealed class ClinicVeterinarianConfiguration : IEntityTypeConfiguration<ClinicVeterinarian>
{
    public void Configure(EntityTypeBuilder<ClinicVeterinarian> builder)
    {
        _ = builder.ToTable("ClinicVeterinarians");

        _ = builder.HasKey(cv => new { cv.ClinicId, cv.VeterinarianId });
    }
}
