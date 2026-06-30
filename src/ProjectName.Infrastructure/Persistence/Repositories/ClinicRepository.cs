using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;

namespace ProjectName.Infrastructure.Persistence.Repositories;

internal sealed class ClinicRepository(AppDbContext dbContext) : IClinicRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> _fieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Clinic, Guid>>)(clinic => clinic.Id),
            ["name"] = (Expression<Func<Clinic, string>>)(clinic => clinic.Name),
            ["address"] = (Expression<Func<Clinic, string>>)(clinic => clinic.Address),
            ["createdAt"] = (Expression<Func<Clinic, DateTimeOffset>>)(clinic => clinic.CreatedAt),
            ["createdAtUtc"] = (Expression<Func<Clinic, DateTimeOffset>>)(clinic => clinic.CreatedAt),
            ["createdBy"] = (Expression<Func<Clinic, string>>)(clinic => clinic.CreatedBy),
            ["updatedAt"] = (Expression<Func<Clinic, DateTimeOffset?>>)(clinic => clinic.UpdatedAt),
            ["updatedAtUtc"] = (Expression<Func<Clinic, DateTimeOffset?>>)(clinic => clinic.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Clinic, string?>>)(clinic => clinic.UpdatedBy)
        };

    public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Clinic? clinic = await dbContext.Clinics
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (clinic is not null)
        {
            await LoadVeterinarianIdsAsync(clinic, cancellationToken);
        }

        return clinic;
    }

    public async Task AddAsync(Clinic clinic, CancellationToken cancellationToken = default)
    {
        _ = await dbContext.Clinics.AddAsync(clinic, cancellationToken);
        _ = await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Clinic clinic, CancellationToken cancellationToken = default)
    {
        await SyncVeterinarianLinksAsync(clinic, cancellationToken);
        _ = dbContext.Clinics.Update(clinic);
        _ = await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Clinics
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Clinics.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Clinic>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Clinic> query = dbContext.Clinics.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, _fieldSelectors, defaultSortField: "name");

        List<Clinic> clinics = await query.ToListAsync(cancellationToken);
        await LoadVeterinarianIdsAsync(clinics, cancellationToken);

        return clinics;
    }

    public async Task<IReadOnlyList<GetClinicResponse>> GetResponsesAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Clinic> query = dbContext.Clinics.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, _fieldSelectors, defaultSortField: "name");

        return await query
            .Select(clinic => new GetClinicResponse(clinic.Id, clinic.Name, clinic.Address))
            .ToListAsync(cancellationToken);
    }

    private async Task LoadVeterinarianIdsAsync(Clinic clinic, CancellationToken cancellationToken)
    {
        List<Guid> veterinarianIds = await dbContext.ClinicVeterinarians
            .Where(link => link.ClinicId == clinic.Id)
            .Select(link => link.VeterinarianId)
            .ToListAsync(cancellationToken);

        clinic.LoadVeterinarianIds(veterinarianIds);
    }

    private async Task LoadVeterinarianIdsAsync(List<Clinic> clinics, CancellationToken cancellationToken)
    {
        foreach (Clinic clinic in clinics)
        {
            await LoadVeterinarianIdsAsync(clinic, cancellationToken);
        }
    }

    private async Task SyncVeterinarianLinksAsync(Clinic clinic, CancellationToken cancellationToken)
    {
        List<ClinicVeterinarian> existingLinks = await dbContext.ClinicVeterinarians
            .Where(link => link.ClinicId == clinic.Id)
            .ToListAsync(cancellationToken);

        List<Guid> desiredVeterinarianIds = [.. clinic.VeterinarianIds];

        List<ClinicVeterinarian> linksToRemove = [.. existingLinks.Where(link => !desiredVeterinarianIds.Contains(link.VeterinarianId))];

        if (linksToRemove.Count > 0)
        {
            dbContext.ClinicVeterinarians.RemoveRange(linksToRemove);
        }

        List<Guid> existingVeterinarianIds = [.. existingLinks.Select(link => link.VeterinarianId)];
        List<ClinicVeterinarian> linksToAdd = [.. desiredVeterinarianIds
            .Where(veterinarianId => !existingVeterinarianIds.Contains(veterinarianId))
            .Select(veterinarianId => new ClinicVeterinarian(clinic.Id, veterinarianId))];

        if (linksToAdd.Count > 0)
        {
            await dbContext.ClinicVeterinarians.AddRangeAsync(linksToAdd, cancellationToken);
        }
    }
}
