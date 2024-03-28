using Microsoft.EntityFrameworkCore;

namespace GenericRepository.EFCore;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Of<TEntity>() where TEntity : class;

    Task<int> CommitChangesAsync(CancellationToken token = default);
}


public class UnitOfWork<TDataContext>(TDataContext context) : IUnitOfWork
    where TDataContext : DbContext
{
    private readonly TDataContext _context = context;

    public IRepository<TEntity> Of<TEntity>() where TEntity : class
    {
        return new Repository<TEntity, TDataContext>(_context);
    }

    public async Task<int> CommitChangesAsync(CancellationToken token = default)
    {
        return await _context.SaveChangesAsync(token);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}