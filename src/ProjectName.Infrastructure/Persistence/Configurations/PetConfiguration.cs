using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Pet entity, specifying table name, primary key, required properties, property conversions, and owned entities for the database context.
/// </summary>
internal sealed class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        _ = builder.ToTable("Pets");

        _ = builder.HasKey(p => p.Id);

        _ = builder.Property(p => p.OwnerId)
            .IsRequired();

        _ = builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(p => p.Species)
            .IsRequired()
            .HasConversion(
                s => s.Value,
                v => PetSpecies.FromValue(v));

        _ = builder.Property(p => p.BirthDate)
            .IsRequired();

        _ = builder.OwnsMany(p => p.VaccineAdministrations, va =>
        {
            _ = va.ToTable("VaccineAdministrations");

            _ = va.WithOwner().HasForeignKey(v => v.PetId);

            _ = va.HasKey(v => v.Id);

            _ = va.Property(v => v.VaccineId)
                .IsRequired();

            _ = va.Property(v => v.VeterinarianId)
                .IsRequired();

            _ = va.Property(v => v.AdministrationOn)
                .IsRequired();
        });
    }
}
