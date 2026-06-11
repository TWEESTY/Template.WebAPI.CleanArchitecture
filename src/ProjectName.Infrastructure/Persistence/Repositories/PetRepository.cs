using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Pets.Common;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;
using System.Linq.Expressions;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class PetRepository(AppDbContext dbContext) : IPetRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Pet, Guid>>)(p => p.Id),
            ["ownerId"] = (Expression<Func<Pet, Guid>>)(p => p.OwnerId),
            ["name"] = (Expression<Func<Pet, string>>)(p => p.Name),
            ["species"] = (Expression<Func<Pet, int>>)(p => p.Species.Value),
            ["birthDate"] = (Expression<Func<Pet, DateOnly>>)(p => p.BirthDate),
            ["createdAt"] = (Expression<Func<Pet, DateTimeOffset>>)(p => p.CreatedAt),
            ["createdBy"] = (Expression<Func<Pet, string>>)(p => p.CreatedBy),
            ["updatedAt"] = (Expression<Func<Pet, DateTimeOffset?>>)(p => p.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Pet, string?>>)(p => p.UpdatedBy)
        };

    public async Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Pets
            .Include(p => p.VaccineAdministrations)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        await dbContext.Pets.AddAsync(pet, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        dbContext.Pets.Update(pet);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Pets
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0;
    }

    public async Task<IReadOnlyList<Pet>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Pet> query = dbContext.Pets.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "name");

        return await query.ToListAsync(cancellationToken);
    }
}
