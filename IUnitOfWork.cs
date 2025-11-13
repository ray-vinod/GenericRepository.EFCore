using Microsoft.EntityFrameworkCore.Storage;

namespace GenericRepository;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Of<TEntity>() where TEntity : class;
    Task<int> SaveChange();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<bool> DatabaseExistsAsync();
}