using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Owners.Common;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;
using System.Linq.Expressions;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class OwnerRepository(AppDbContext dbContext) : IOwnerRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Owner, Guid>>)(o => o.Id),
            ["firstName"] = (Expression<Func<Owner, string>>)(o => o.FirstName),
            ["lastName"] = (Expression<Func<Owner, string>>)(o => o.LastName),
            ["email"] = (Expression<Func<Owner, string>>)(o => o.Email),
            ["phoneNumber"] = (Expression<Func<Owner, string>>)(o => o.PhoneNumber),
            ["createdAt"] = (Expression<Func<Owner, DateTimeOffset>>)(o => o.CreatedAt),
            ["createdBy"] = (Expression<Func<Owner, string>>)(o => o.CreatedBy),
            ["updatedAt"] = (Expression<Func<Owner, DateTimeOffset?>>)(o => o.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Owner, string?>>)(o => o.UpdatedBy)
        };

    public async Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Owners.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task AddAsync(Owner owner, CancellationToken cancellationToken = default)
    {
        await dbContext.Owners.AddAsync(owner, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Owner owner, CancellationToken cancellationToken = default)
    {
        dbContext.Owners.Update(owner);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Owners.Where(o => o.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    public async Task<IReadOnlyList<Owner>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Owner> query = dbContext.Owners.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "lastName");

        return await query.ToListAsync(cancellationToken);
    }
}
