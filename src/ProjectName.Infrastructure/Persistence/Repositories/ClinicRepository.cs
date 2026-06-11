using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class ClinicRepository(AppDbContext dbContext) : IClinicRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Clinic, Guid>>)(clinic => clinic.Id),
            ["name"] = (Expression<Func<Clinic, string>>)(clinic => clinic.Name),
            ["address"] = (Expression<Func<Clinic, string>>)(clinic => clinic.Address),
            ["createdAt"] = (Expression<Func<Clinic, DateTimeOffset>>)(clinic => clinic.CreatedAt),
            ["createdBy"] = (Expression<Func<Clinic, string>>)(clinic => clinic.CreatedBy),
            ["updatedAt"] = (Expression<Func<Clinic, DateTimeOffset?>>)(clinic => clinic.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Clinic, string?>>)(clinic => clinic.UpdatedBy)
        };

    public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Clinics
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Clinic clinic, CancellationToken cancellationToken = default)
    {
        await dbContext.Clinics.AddAsync(clinic, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Clinic clinic, CancellationToken cancellationToken = default)
    {
        dbContext.Clinics.Update(clinic);
        await dbContext.SaveChangesAsync(cancellationToken);
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
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "name");

        return await query.ToListAsync(cancellationToken);
    }
}
