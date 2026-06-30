using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Owner entity, specifying table name, primary key, and required properties for the database context.
/// </summary>
internal sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        _ = builder.ToTable("Owners");

        _ = builder.HasKey(o => o.Id);

        _ = builder.Property(o => o.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(o => o.LastName)
            .IsRequired()
            .HasMaxLength(100);

        _ = builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(200);

        _ = builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
    }
}
