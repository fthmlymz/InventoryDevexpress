using Domain.Common;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity;
        Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void SaveChanges();
        Task Rollback();
    }
}
