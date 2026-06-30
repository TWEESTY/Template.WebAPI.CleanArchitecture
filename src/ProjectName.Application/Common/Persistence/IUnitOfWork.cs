namespace ProjectName.Application.Common.Persistence;

/// <summary>
/// Represents a unit of work that can be used to manage transactions and persistence in the application.
/// More information about the Unit of Work pattern can be found at https://medium.com/@chausse.nicolas/sharing-transaction-between-services-in-asp-net-core-real-world-business-website-with-ef-core-41c337966f9e.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    Task StartAsync();
    Task EndAsync(bool forceRollback = false);
}
