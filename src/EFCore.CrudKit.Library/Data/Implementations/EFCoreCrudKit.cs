using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Implementations
{
    public class EFCoreCrudKit<TContext>(TContext context) : IEFCoreCrudKit where TContext : DbContext
    {
        private readonly TContext _context = context;

        /// <summary>
        /// Insert a single object of type <paramref name="TEntity"/> into the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task InsertAsync<TEntity>(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellation);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Insert a bulk objects of type <paramref name="TEntity"/> into the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task InsertRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cancellation);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Updates a single <paramref name="TEntity"/> object in the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task UpdateAsync<TEntity>(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().Update(entity);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Updates bulk <paramref name="TEntity"/> objects in the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task UpdateRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().UpdateRange(entities);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Deletes a single <paramref name="TEntity"/> object from the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task DeleteAsync<TEntity>(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().Remove(entity);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Deletes bulk <paramref name="TEntity"/> objects from the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task DeleteRangeAsync<TEntity>(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().RemoveRange(entities);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Toggles the deprecation a single <paramref name="TEntity"/> object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task ToggleAsync<TEntity>(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).Property(x => x.IsDeprecated).IsModified = true;
            entity.IsDeprecated = !entity.IsDeprecated;
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Toggles the deprecations a range of <paramref name="TEntity"/> objects
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task ToggleAsync<TEntity>(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default) where TEntity : EntityBase
        {
            _context.Set<TEntity>().AttachRange(entities);
            foreach (TEntity entity in entities)
            {
                _context.Entry(entity).Property(x => x.IsDeprecated).IsModified = true;
                entity.IsDeprecated = !entity.IsDeprecated;
            }
            
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        /// <summary>
        /// Gets a single <paramref name="TEntity"/> object from the data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<TEntity?> FindByIdAsync<TEntity>(Guid id, bool trackChanges) where TEntity : EntityBase =>
            trackChanges ?
                await _context.Set<TEntity>().FirstOrDefaultAsync(i => i.Id.Equals(id)) :
                await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(i => i.Id.Equals(id));

        /// <summary>
        /// Gets <paramref name="TEntity"/> as Queryable objects
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<TEntity> AsQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, bool trackChanges) where TEntity : EntityBase
            => !trackChanges ? 
                _context.Set<TEntity>().Where(predicate).AsNoTracking() : 
                _context.Set<TEntity>().Where(predicate);

        /// <summary>
        /// Gets the number of <paramref name="TEntity"/> objects that satisfy the condition
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
            => await _context.Set<TEntity>().CountAsync(predicate);

        /// <summary>
        /// Checks whether or not the <paramref name="TEntity"/> object exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
            => await _context.Set<TEntity>().AnyAsync(predicate);

        /// <summary>
        /// Saves all changes made in this context to the data store
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<int> SaveAsync(CancellationToken cancellation = default) =>
            await _context.SaveChangesAsync(cancellation);
    }
}
