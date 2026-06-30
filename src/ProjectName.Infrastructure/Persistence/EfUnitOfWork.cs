using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectName.Application.Common.Persistence;

namespace ProjectName.Infrastructure.Persistence;

/// <summary>
/// Represents a unit of work for managing database transactions using Entity Framework Core. This class is not thread-safe like DbContext.
/// </summary>
/// <param name="unitOfWorkManager">The unit of work manager.</param>
/// <param name="dbContext">The database context.</param>
/// <param name="isParent">Indicates whether this unit of work is a parent unit of work.</param>
public class EfUnitOfWork(IUnitOfWorkManager unitOfWorkManager, DbContext dbContext, bool isParent = false) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext;
    private IDbContextTransaction? _currentTransaction;
    private readonly bool _isParent = isParent;
    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

    public async Task StartAsync()
    {
        if (_isParent && _dbContext.Database.CurrentTransaction == null)
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
        }
    }

    public Task EndAsync(bool forceRollback = false)
    {
        if (forceRollback)
        {
            return DoRollbackIfNecessaryAsync();
        }

        return DoCommitIfNecessaryAsync();

    }

    private async Task DoRollbackIfNecessaryAsync()
    {
        if (_isParent && _currentTransaction != null)
        {
            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                _dbContext.ChangeTracker.Clear();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    private async Task DoCommitIfNecessaryAsync()
    {
        if (!_isParent || _currentTransaction == null)
        {
            return;
        }

        try
        {
            await _currentTransaction.CommitAsync();
        }
        catch
        {
            // TODO logging
            _currentTransaction?.Rollback();
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        finally
        {
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _unitOfWorkManager.EndUnitOfWorkAsync(this, forceRollback: true);
        }

        _currentTransaction?.Dispose();

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }
}
