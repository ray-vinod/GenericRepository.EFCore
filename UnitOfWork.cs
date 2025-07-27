using Microsoft.EntityFrameworkCore;

namespace GenericRepository;

public class UnitOfWork<TDataContext>(TDataContext context) : IUnitOfWork where TDataContext : DbContext
{
    private readonly TDataContext _context = context ?? throw new ArgumentNullException(nameof(context));


    public IRepository<TEntity> Of<TEntity>() where TEntity : class
        => new Repository<TEntity, TDataContext>(_context);

    public Task<int> SaveChange() => _context.SaveChangesAsync();


    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var timeStamp = DateTime.UtcNow;

        foreach (var entry in _context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = timeStamp;
                    entry.Entity.UpdatedAt = timeStamp;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = timeStamp;
                    break;
                case EntityState.Unchanged:
                    break;
            }
        }

        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose() => _context.Dispose();
}