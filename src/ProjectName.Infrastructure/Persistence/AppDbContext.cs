using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context, providing access to the database and managing entity configurations, audit metadata, and change tracking for the application's entities.
/// </summary>
/// <param name="options"></param>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    private const string _systemUser = "system-seed";

    public DbSet<ClinicVeterinarian> ClinicVeterinarians { get; set; } = null!;
    public DbSet<Clinic> Clinics { get; set; } = null!;
    public DbSet<Veterinarian> Veterinarians { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Owner> Owners { get; set; } = null!;
    public DbSet<Vaccine> Vaccines { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
#pragma warning disable CA1062 // Validate arguments of public methods
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
#pragma warning restore CA1062 // Validate arguments of public methods
        ApplyUtcDateTimeOffsetConversions(modelBuilder);
    }

    private static void ApplyUtcDateTimeOffsetConversions(ModelBuilder modelBuilder)
    {
        ValueConverter<DateTimeOffset, DateTime> dateTimeOffsetConverter = new(
            v => v.UtcDateTime,
            v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)));

        ValueConverter<DateTimeOffset?, DateTime?> nullableDateTimeOffsetConverter = new(
            v => v.HasValue ? v.Value.UtcDateTime : null,
            v => v.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : null);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(dateTimeOffsetConverter);
                    continue;
                }

                if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(nullableDateTimeOffsetConverter);
                }
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditMetadata();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditMetadata();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditMetadata();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditMetadata();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditMetadata()
    {
        DateTimeOffset now = TimeProvider.System.GetUtcNow();

        IEnumerable<EntityEntry<EntityBase>> entries = ChangeTracker
            .Entries<EntityBase>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (EntityEntry<EntityBase> entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedAt).CurrentValue = now;

                if (string.IsNullOrWhiteSpace(entry.Property(e => e.CreatedBy).CurrentValue))
                {
                    entry.Property(e => e.CreatedBy).CurrentValue = _systemUser;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.UpdatedAt).CurrentValue = now;

                if (string.IsNullOrWhiteSpace(entry.Property(e => e.UpdatedBy).CurrentValue))
                {
                    entry.Property(e => e.UpdatedBy).CurrentValue = _systemUser;
                }
            }
        }
    }
}
