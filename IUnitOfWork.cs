using Microsoft.EntityFrameworkCore;

namespace GenericRepository;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Of<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken token = default);
}