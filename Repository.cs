using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(object id);

    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> GetByIdAsync(object id);

    Task<List<TEntity>?> GetItemsAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    IQueryable<TEntity> QueryableEntity();
}


public class Repository<TEntity, TDataContext>(TDataContext context) : IRepository<TEntity>
      where TEntity : class
      where TDataContext : DbContext
{
    protected readonly TDataContext _context = context;
    internal DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);

        _dbSet.Remove(entity);
        _context.Entry(entity).State = EntityState.Deleted;

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(object id)
    {
        TEntity? entity = await _dbSet.FindAsync(id);
        return await DeleteAsync(entity!);
    }

    public async Task<TEntity> GetByIdAsync(object id)
    {
        TEntity? entity = await _dbSet.FindAsync(id);
        return entity!;
    }

    public async Task<List<TEntity>?> GetItemsAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        try
        {
            IQueryable<TEntity>? query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return orderBy == null
                ? await query.ToListAsync()
                : await orderBy(query).ToListAsync();
        }
        catch (Exception)
        {
            return null!;
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var dbSet = _context.Set<TEntity>();
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }

    public virtual IQueryable<TEntity> QueryableEntity()
    {
        IQueryable<TEntity>? query = _dbSet;
        return query.AsQueryable();
    }
}
