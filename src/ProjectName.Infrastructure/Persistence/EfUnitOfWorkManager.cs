using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Common.Persistence;

namespace ProjectName.Infrastructure.Persistence;

public class EfUnitOfWorkManager : IUnitOfWorkManager
{
    private int _numberOfUnitOfWork = 0;
    private readonly DbContext _dbContext;

    public EfUnitOfWorkManager(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

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