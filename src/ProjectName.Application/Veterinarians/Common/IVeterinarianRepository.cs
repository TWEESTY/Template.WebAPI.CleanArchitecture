using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Veterinarians.Common;

public interface IVeterinarianRepository
{
    Task<Veterinarian?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default);
    Task UpdateAsync(Veterinarian veterinarian, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Veterinarian>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
