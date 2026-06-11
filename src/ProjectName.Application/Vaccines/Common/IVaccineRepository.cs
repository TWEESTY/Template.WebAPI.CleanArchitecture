using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Vaccines.Common;

public interface IVaccineRepository
{
    Task<Vaccine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Vaccine vaccine, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vaccine vaccine, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vaccine>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
