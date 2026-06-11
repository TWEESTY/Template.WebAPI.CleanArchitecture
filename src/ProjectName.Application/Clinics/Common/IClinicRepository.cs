using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;

namespace ProjectName.Application.Clinics.Common;

public interface IClinicRepository
{
    Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Clinic clinic, CancellationToken cancellationToken = default);
    Task UpdateAsync(Clinic clinic, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Clinic>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default);
}
