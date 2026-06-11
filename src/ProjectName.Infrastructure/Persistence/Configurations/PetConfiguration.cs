using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;
using ProjectName.Domain.Enums;

namespace ProjectName.Infrastructure.Persistence.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Species)
            .IsRequired()
            .HasConversion(
                s => s.Value,
                v => PetSpecies.FromValue(v));

        builder.Property(p => p.BirthDate)
            .IsRequired();

        builder.OwnsMany(p => p.VaccineAdministrations, va =>
        {
            va.ToTable("VaccineAdministrations");

            va.WithOwner().HasForeignKey(v => v.PetId);

            va.HasKey(v => v.Id);

            va.Property(v => v.VaccineId)
                .IsRequired();

            va.Property(v => v.VeterinarianId)
                .IsRequired();

            va.Property(v => v.AdministrationOn)
                .IsRequired();
        });
    }
}
