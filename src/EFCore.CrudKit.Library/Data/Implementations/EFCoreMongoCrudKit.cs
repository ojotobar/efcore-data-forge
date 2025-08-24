using EFCore.CrudKit.Library.Data.Interfaces;
using EFCore.CrudKit.Library.Models;
using EFCore.CrudKit.Library.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace EFCore.CrudKit.Library.Data.Implementations
{
    public sealed class EFCoreMongoCrudKit : IEFCoreMongoCrudKit
    {
        private readonly IMongoDatabase _database;
        public EFCoreMongoCrudKit(EFCoreDataForgeOptions options)
        {
            var settings = options.MongoDb;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        internal EFCoreMongoCrudKit(IMongoDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Inserts a single <paramref name="TCollection"/> document into the collection
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="document"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task InsertAsync<TCollection>(TCollection document,
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.InsertOneAsync(document, cancellationToken: token);
        }

        /// <summary>
        /// Inserts many <paramref name="TCollection"/> documents into the collection
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task InsertRangeAsync<TCollection>(List<TCollection> documents, 
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.InsertManyAsync(documents, cancellationToken: token);
        }

        /// <summary>
        /// Replaces a single <paramref name="TCollection"/> document in the collection
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="document"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ReplaceAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, 
            TCollection document, CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.ReplaceOneAsync<TCollection>(predicate, document, cancellationToken: token);
        }

        /// <summary>
        /// Updates a single <paramref name="TCollection"/> document
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="document"></param>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task UpdateOneAsync<TCollection>(TCollection document, Expression<Func<TCollection, bool>> predicate, 
            CancellationToken token = default) where TCollection : class
        {
            var filter = Builders<TCollection>.Filter.Where(predicate);

            var updates = GetUpdateDefinitions(document);
            var updateDef = Builders<TCollection>.Update.Combine(updates);

            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.UpdateOneAsync(filter, updateDef, cancellationToken: token);
        }

        /// <summary>
        /// Delets a single <paramref name="TCollection"/> document based on the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeleteAsync<TCollection>(Expression<Func<TCollection, bool>> predicate,
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.DeleteOneAsync(predicate, token);
        }

        /// <summary>
        /// Delets many <paramref name="TCollection"/> documents based on the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task DeleteRangeAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, 
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            await collection.DeleteManyAsync(predicate, token);
        }

        /// <summary>
        /// Gets a single <paramref name="TCollection"/> document based on the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TCollection?> FindOneAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, 
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            return await collection.Find(predicate).FirstOrDefaultAsync(token);
        }

        /// <summary>
        /// Gets a list of <paramref name="TCollection"/> documents based on the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<TCollection>> FindAsync<TCollection>(Expression<Func<TCollection, bool>> predicate,
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            return await collection.Find(predicate).ToListAsync(token);
        }

        /// <summary>
        /// <paramref name="TCollection"/> as Queryable documents
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<TCollection> AsQueryable<TCollection>(Expression<Func<TCollection, bool>> predicate) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            return collection.AsQueryable().Where(predicate);
        }

        /// <summary>
        /// Gets the number of <typeparamref name="TCollection"/> documents that satisfies the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<long> CountAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, 
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            return await collection.CountDocumentsAsync(predicate, cancellationToken: token);
        }

        /// <summary>
        /// Checks whether or not any <paramref name="TCollection"/> document exists based on the predicate
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<TCollection>(Expression<Func<TCollection, bool>> predicate, 
            CancellationToken token = default) where TCollection : class
        {
            var collection = _database.GetCollection<TCollection>(typeof(TCollection).Name);
            return await (await collection.FindAsync(predicate, cancellationToken: token)).AnyAsync(token);
        }

        private List<UpdateDefinition<TCollection>> GetUpdateDefinitions<TCollection>(TCollection document)
        {
            var bsonDoc = document.ToBsonDocument();

            var updates = new List<UpdateDefinition<TCollection>>();
            foreach (var elem in bsonDoc.Elements)
            {
                if (elem.Name == "Id" || elem.Name == "_id") continue; // avoid modifying Id
                updates.Add(Builders<TCollection>.Update.Set(elem.Name, elem.Value));
            }

            return updates;
        }
    }
}