using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class VeterinarianRepository(AppDbContext dbContext) : IVeterinarianRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Veterinarian, Guid>>)(v => v.Id),
            ["firstName"] = (Expression<Func<Veterinarian, string>>)(v => v.FirstName),
            ["lastName"] = (Expression<Func<Veterinarian, string>>)(v => v.LastName),
            ["email"] = (Expression<Func<Veterinarian, string>>)(v => v.Email),
            ["licenseNumber"] = (Expression<Func<Veterinarian, string>>)(v => v.LicenseNumber),
            ["createdAt"] = (Expression<Func<Veterinarian, DateTimeOffset>>)(v => v.CreatedAt),
            ["createdBy"] = (Expression<Func<Veterinarian, string>>)(v => v.CreatedBy),
            ["updatedAt"] = (Expression<Func<Veterinarian, DateTimeOffset?>>)(v => v.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Veterinarian, string?>>)(v => v.UpdatedBy)
        };

    public async Task<Veterinarian?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Veterinarians.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task AddAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default)
    {
        await dbContext.Veterinarians.AddAsync(veterinarian, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default)
    {
        dbContext.Veterinarians.Update(veterinarian);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Veterinarians.Where(v => v.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    public async Task<IReadOnlyList<Veterinarian>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Veterinarian> query = dbContext.Veterinarians.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "lastName");

        return await query.ToListAsync(cancellationToken);
    }
}
