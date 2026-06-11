using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectName.Infrastructure.Persistence.Configurations;

public class ClinicVeterinarianConfiguration : IEntityTypeConfiguration<ClinicVeterinarian>
{
    public void Configure(EntityTypeBuilder<ClinicVeterinarian> builder)
    {
        builder.ToTable("ClinicVeterinarians");

        builder.HasKey(cv => new { cv.ClinicId, cv.VeterinarianId });
    }
}
