using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Search;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Common.Search;

namespace ProjectName.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository(AppDbContext dbContext) : IAppointmentRepository
{
    private static readonly IReadOnlyDictionary<string, LambdaExpression> FieldSelectors =
        new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = (Expression<Func<Appointment, Guid>>)(a => a.Id),
            ["petId"] = (Expression<Func<Appointment, Guid>>)(a => a.PetId),
            ["veterinarianId"] = (Expression<Func<Appointment, Guid>>)(a => a.VeterinarianId),
            ["clinicId"] = (Expression<Func<Appointment, Guid>>)(a => a.ClinicId),
            ["startAtUtc"] = (Expression<Func<Appointment, DateTime>>)(a => a.StartAtUtc),
            ["endAtUtc"] = (Expression<Func<Appointment, DateTime>>)(a => a.EndAtUtc),
            ["reason"] = (Expression<Func<Appointment, string>>)(a => a.Reason),
            ["status"] = (Expression<Func<Appointment, int>>)(a => a.Status.Value),
            ["createdAt"] = (Expression<Func<Appointment, DateTimeOffset>>)(a => a.CreatedAt),
            ["createdBy"] = (Expression<Func<Appointment, string>>)(a => a.CreatedBy),
            ["updatedAt"] = (Expression<Func<Appointment, DateTimeOffset?>>)(a => a.UpdatedAt),
            ["updatedBy"] = (Expression<Func<Appointment, string?>>)(a => a.UpdatedBy)
        };

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await dbContext.Appointments.AddAsync(appointment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        dbContext.Appointments.Update(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int deletedCount = await dbContext.Appointments.Where(a => a.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    public async Task<IReadOnlyList<Appointment>> GetAsync(SearchParameters? searchParameters = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Appointment> query = dbContext.Appointments.AsNoTracking();
        query = SearchQueryHelper.Apply(query, searchParameters, FieldSelectors, defaultSortField: "startAtUtc");

        return await query.ToListAsync(cancellationToken);
    }
}
