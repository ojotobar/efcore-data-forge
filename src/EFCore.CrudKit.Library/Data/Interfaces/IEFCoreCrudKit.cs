using EFCore.CrudKit.Library.Models;
using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface IEFCoreCrudKit
    {
        IQueryable<TEntity> AsQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, bool trackChanges) where TEntity : EntityBase;
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;
        Task DeleteAsync<TEntity>(TEntity entity, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task DeleteRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase;
        Task<TEntity?> FindByIdAsync<TEntity>(Guid id, bool trackChanges) where TEntity : EntityBase;
        Task InsertAsync<TEntity>(TEntity entity, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task InsertRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task<int> SaveAsync(CancellationToken cancellation = default);
        Task ToggleAsync<TEntity>(TEntity entity, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task ToggleAsync<TEntity>(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task UpdateAsync<TEntity>(TEntity entity, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
        Task UpdateRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true, CancellationToken cancellation = default) where TEntity : EntityBase;
    }
}
