using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Implementations
{
    public class EFCoreCrudKit<TEntity, TContext>(TContext context) : 
        IEFCoreCrudKit<TEntity> where TEntity : EntityBase where TContext : DbContext
    {
        private readonly TContext _context = context;

        public async Task InsertAsync(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellation);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task InsertRangeAsync(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cancellation);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task UpdateAsync(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            _context.Set<TEntity>().Update(entity);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task UpdateRangeAsync(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task DeleteAsync(TEntity entity, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            _context.Set<TEntity>().Remove(entity);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task DeleteRangeAsync(List<TEntity> entities, bool saveNow = true,
            CancellationToken cancellation = default)
        {
            _context.Set<TEntity>().RemoveRange(entities);
            if (saveNow)
            {
                await SaveAsync(cancellation);
            }
        }

        public async Task<TEntity?> FindByIdAsync(Guid id, bool trackChanges) =>
            trackChanges ?
                await _context.Set<TEntity>().FirstOrDefaultAsync(i => i.Id.Equals(id)) :
                await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(i => i.Id.Equals(id));

        public IQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate, bool trackChanges) =>
            !trackChanges ? _context.Set<TEntity>()
                    .Where(predicate)
                    .AsNoTracking() : _context.Set<TEntity>()
                    .Where(predicate);

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) =>
            await _context.Set<TEntity>().CountAsync(predicate);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) =>
            await _context.Set<TEntity>().AnyAsync(predicate);

        public async Task<int> SaveAsync(CancellationToken cancellation = default) =>
            await _context.SaveChangesAsync(cancellation);
    }
}
