using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence.Configurations;

public class VeterinarianConfiguration : IEntityTypeConfiguration<Veterinarian>
{
    public void Configure(EntityTypeBuilder<Veterinarian> builder)
    {
        builder.ToTable("Veterinarians");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);
    }
}
