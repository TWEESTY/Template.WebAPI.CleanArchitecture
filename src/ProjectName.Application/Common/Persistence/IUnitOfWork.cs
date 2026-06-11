namespace ProjectName.Application.Common.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    Task StartAsync();
    Task EndAsync(bool forceRollback = false);
}