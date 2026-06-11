using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;
using System.Linq.Expressions;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class VaccineRepository(AppDbContext dbContext) : IVaccineRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Vaccine, Guid>>)(v => v.Id),
            ["code"] = (Expression<Func<Vaccine, string>>)(v => v.Code),
            ["name"] = (Expression<Func<Vaccine, string>>)(v => v.Name),
            ["createdAt"] = (Expression<Func<Vaccine, DateTimeOffset>>)(v => v.CreatedAt),
            ["createdBy"] = (Expression<Func<Vaccine, string>>)(v => v.CreatedBy),
            ["updatedAt"] = (Expression<Func<Vaccine, DateTimeOffset?>>)(v => v.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Vaccine, string?>>)(v => v.UpdatedBy)
        };

    public async Task<Vaccine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Vaccines.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task AddAsync(Vaccine vaccine, CancellationToken cancellationToken = default)
    {
        await dbContext.Vaccines.AddAsync(vaccine, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Vaccine vaccine, CancellationToken cancellationToken = default)
    {
        dbContext.Vaccines.Update(vaccine);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Vaccines.Where(v => v.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    public async Task<IReadOnlyList<Vaccine>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Vaccine> query = dbContext.Vaccines.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "name");

        return await query.ToListAsync(cancellationToken);
    }
}
