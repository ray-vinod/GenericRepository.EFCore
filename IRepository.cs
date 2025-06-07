using System.Linq.Expressions;

namespace GenericRepository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(params object[] id);
    Task<IEnumerable<TEntity>?> GetAllAsync(bool includeAuditable = true);

    Task<IEnumerable<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>> predicate, bool includeAuditable = true, params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeAuditable = true, params Expression<Func<TEntity, object>>[] includes);

    IQueryable<TEntity> AsQueryable(bool includeAuditable = true);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(params object[] id);
    Task DeleteAsync(TEntity entity);
    Task SoftDeleteAsync(TEntity entity);
    Task RestoreAsync(TEntity entity);

    Task<PagedList<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool includeAuditable = true, params Expression<Func<TEntity, object>>[] includes);
}