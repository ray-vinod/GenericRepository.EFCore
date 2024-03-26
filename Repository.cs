using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetEntity();

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> GetByIdAsync(object id);
    Task<TEntity?> GetByNameAsync(string name);
    Task<List<TEntity>?> GetAllAsync();

    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> AddRangeAsync(TEntity[] entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> DeleteAsync(object id);
    Task<TEntity> DeleteAsync(TEntity entity);
}


public class Repository<TEntity, TDataContext>(TDataContext context)
    : IRepository<TEntity>
    where TEntity : class
    where TDataContext : DbContext
{
    protected readonly TDataContext _context = context
        ?? throw new ArgumentNullException(nameof(context));

    internal DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity> AddRangeAsync(TEntity[] entity)
    {
        await _dbSet.AddRangeAsync(entity);
        return entity.Last();
    }

    public async Task<TEntity> DeleteAsync(object id)
    {
        TEntity? entity = await _dbSet.FindAsync(id);
        return await DeleteAsync(entity!);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
        _context.Entry(entity).State = EntityState.Deleted;

        return await Task.FromResult(entity);
    }

    public IQueryable<TEntity> GetEntity()
    {
        IQueryable<TEntity>? query = _dbSet;
        return query.AsQueryable();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        try
        {
            IQueryable<TEntity>? query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<TEntity>?> GetAllAsync()
    {
        return await GetEntity().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity?> GetByNameAsync(string name)
    {
        try
        {
            if (_dbSet.Where(e => EF.Property<bool>(e, "Name")) != null)
            {
                var entity = _dbSet.FirstOrDefault(entity => EF.Property<string>(entity, "Name") == name);
                return await Task.FromResult(entity);
            }

            throw new InvalidOperationException($"Entity {nameof(TEntity)} has not Name property");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var dbSet = _context.Set<TEntity>();
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }
}