using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository;
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Entity();

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> FindByIdAsync(object id);
    Task<TEntity?> FindByNameAsync(string name);
    Task<List<TEntity>?> RecordsAsync();

    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> AddRangeAsync(TEntity[] entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<TEntity> RemoveAsync(object id);
    Task<TEntity> RemoveAsync(TEntity entity);
    Task<TEntity> RemoveRangeAsync(TEntity[] entity);
}