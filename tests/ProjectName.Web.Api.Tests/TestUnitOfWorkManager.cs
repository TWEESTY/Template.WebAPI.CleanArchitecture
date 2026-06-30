using Microsoft.EntityFrameworkCore.Storage;
using ProjectName.Application.Common.Persistence;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Web.Api.Tests;

internal sealed class TestUnitOfWorkManager(AppDbContext dbContext) : IUnitOfWorkManager
{
    private readonly AppDbContext _dbContext = dbContext;
    private int _activeScopes;

    public async Task<IUnitOfWork> StartOneUnitOfWorkAsync()
    {
        bool isParent = _activeScopes++ == 0;
        TestUnitOfWork unitOfWork = new(this, _dbContext, isParent);
        await unitOfWork.StartAsync();
        return unitOfWork;
    }

    public async Task EndUnitOfWorkAsync(IUnitOfWork unitOfWork, bool forceRollback = false)
    {
        await unitOfWork.EndAsync(forceRollback);
    }

    private void DecreaseDepth()
    {
        _activeScopes = Math.Max(0, _activeScopes - 1);
    }

    private sealed class TestUnitOfWork(TestUnitOfWorkManager manager, AppDbContext dbContext, bool isParent) : IUnitOfWork
    {
        private readonly TestUnitOfWorkManager _manager = manager;
        private readonly AppDbContext _dbContext = dbContext;
        private readonly bool _isParent = isParent;

        private IDbContextTransaction? _transaction;
        private bool _isCompleted;

        public async Task StartAsync()
        {
            if (_isParent && _dbContext.Database.CurrentTransaction is null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task EndAsync(bool forceRollback = false)
        {
            if (_isCompleted)
            {
                return;
            }

            try
            {
                if (_isParent && _transaction is not null)
                {
                    if (forceRollback)
                    {
                        await _transaction.RollbackAsync();
                        _dbContext.ChangeTracker.Clear();
                    }
                    else
                    {
                        _ = await _dbContext.SaveChangesAsync();
                        await _transaction.CommitAsync();
                    }
                }
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                }

                _isCompleted = true;
                _manager.DecreaseDepth();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
            {
                await EndAsync(forceRollback: true);
            }
        }
    }
}
