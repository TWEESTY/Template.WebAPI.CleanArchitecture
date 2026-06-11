using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectName.Domain.Entities;

namespace ProjectName.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    private const string SystemUser = "system-seed";

    public DbSet<ClinicVeterinarian> ClinicVeterinarians { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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
        DateTimeOffset now = DateTimeOffset.UtcNow;

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
                    entry.Property(e => e.CreatedBy).CurrentValue = SystemUser;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.UpdatedAt).CurrentValue = now;

                if (string.IsNullOrWhiteSpace(entry.Property(e => e.UpdatedBy).CurrentValue))
                {
                    entry.Property(e => e.UpdatedBy).CurrentValue = SystemUser;
                }
            }
        }
    }
}
