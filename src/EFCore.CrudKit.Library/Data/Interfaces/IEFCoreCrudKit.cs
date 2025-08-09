using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface IEFCoreCrudKit<TEntity>
    {
        IQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate, bool trackChanges);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TEntity entity, bool saveNow = true, CancellationToken cancellation = default);
        Task DeleteRangeAsync(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FindByIdAsync(Guid id, bool trackChanges);
        Task InsertAsync(TEntity entity, bool saveNow = true, CancellationToken cancellation = default);
        Task InsertRangeAsync(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default);
        Task<int> SaveAsync(CancellationToken cancellation = default);
        Task UpdateAsync(TEntity entity, bool saveNow = true, CancellationToken cancellation = default);
        Task UpdateRangeAsync(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default);
    }
}
