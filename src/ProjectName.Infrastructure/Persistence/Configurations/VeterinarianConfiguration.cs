using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Veterinarian entity, specifying table name, primary key, and required properties for the database context.
/// </summary>
internal sealed class VeterinarianConfiguration : IEntityTypeConfiguration<Veterinarian>
{
    public void Configure(EntityTypeBuilder<Veterinarian> builder)
    {
        _ = builder.ToTable("Veterinarians");

        _ = builder.HasKey(v => v.Id);

        _ = builder.Property(v => v.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(v => v.LastName)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(v => v.Email)
            .IsRequired()
            .HasMaxLength(200);

        _ = builder.Property(v => v.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);
    }
}
