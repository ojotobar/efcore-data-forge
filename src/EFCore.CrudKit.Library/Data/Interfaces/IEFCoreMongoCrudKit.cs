using EFCore.CrudKit.Library.Models;
using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface IEFCoreMongoCrudKit
    {
        IQueryable<TCollection> AsQueryable<TCollection>(Expression<Func<TCollection, bool>> predicate) where TCollection : MongoEntityBase;
        Task<long> CountAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task DeleteAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task DeleteRangeAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task<bool> ExistsAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task<List<TCollection>> FindAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task<TCollection?> FindOneAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task InsertAsync<TCollection>(TCollection document, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task InsertRangeAsync<TCollection>(List<TCollection> documents, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task ReplaceAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, TCollection document, CancellationToken token = default) where TCollection : MongoEntityBase;
        Task UpdateOneAsync<TCollection>(TCollection document, Expression<Func<TCollection, bool>> predicate, CancellationToken token = default) where TCollection : MongoEntityBase;
    }
}
