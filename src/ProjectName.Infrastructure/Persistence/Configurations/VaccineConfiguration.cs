using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Vaccine entity, specifying table name, primary key, and required properties for the database context.
/// </summary>
internal sealed class VaccineConfiguration : IEntityTypeConfiguration<Vaccine>
{
    public void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        _ = builder.ToTable("Vaccines");

        _ = builder.HasKey(v => v.Id);

        _ = builder.Property(v => v.Code)
            .IsRequired()
            .HasMaxLength(50);

        _ = builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
