using Microsoft.EntityFrameworkCore;

namespace GenericRepository.EFCore;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Of<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken token = default);
}

public class UnitOfWork<TDataContext>(TDataContext context) : IUnitOfWork
    where TDataContext : DbContext
{
    private readonly TDataContext _context = context;

    public IRepository<TEntity> Of<TEntity>() where TEntity : class => new Repository<TEntity, TDataContext>(_context);

    public int SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync(CancellationToken token = default) => await _context.SaveChangesAsync(token);

    public void Dispose() => _context.Dispose();
}