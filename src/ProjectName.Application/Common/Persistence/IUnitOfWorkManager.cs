namespace ProjectName.Application.Common.Persistence;

public interface IUnitOfWorkManager
{
    Task<IUnitOfWork> StartOneUnitOfWorkAsync();

    Task EndUnitOfWorkAsync(IUnitOfWork unitOfWork, bool forceRollback = false);
}
