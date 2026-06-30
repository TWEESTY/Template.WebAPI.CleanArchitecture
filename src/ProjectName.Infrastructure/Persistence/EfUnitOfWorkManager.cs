using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Persistence;

namespace ProjectName.Infrastructure.Persistence;

/// <summary>
/// Manages the lifecycle of units of work for Entity Framework Core, ensuring proper transaction handling and disposal.
/// </summary>
/// <param name="dbContext">The database context.</param>
internal sealed class EfUnitOfWorkManager(DbContext dbContext) : IUnitOfWorkManager
{
    private int _numberOfUnitOfWork;
    private readonly DbContext _dbContext = dbContext;

    public Task EndUnitOfWorkAsync(IUnitOfWork unitOfWork, bool forceRollback = false)
    {
        _numberOfUnitOfWork--;
        return unitOfWork.EndAsync(forceRollback);
    }

    public async Task<IUnitOfWork> StartOneUnitOfWorkAsync()
    {
        IUnitOfWork unitOfWork = new EfUnitOfWork(this, _dbContext, isParent: _numberOfUnitOfWork++ == 0);

        try
        {
            await unitOfWork.StartAsync();
            return unitOfWork;
        }
        catch
        {
            await unitOfWork.DisposeAsync();
            throw;
        }
    }
}
