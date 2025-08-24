using Dapper;
using EFCore.CrudKit.Library.Data.Interfaces;

namespace EFCore.CrudKit.Library.Data.Implementations
{
    public class DataForgeRawCrudKit : IDataForgeRawCrudKit
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public DataForgeRawCrudKit(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Executes a query and returns the first matching record or <c>null</c> if none are found.
        /// </summary>
        /// <typeparam name="T">The type to map the query result to.</typeparam>
        /// <param name="query">The SQL query string to execute.</param>
        /// <param name="param">Optional query parameters (e.g., anonymous object or dictionary).</param>
        /// <returns>
        /// The first matching record mapped to <typeparamref name="T"/> or <c>null</c> if no result exists.
        /// </returns>
        public async Task<T?> FindOneAsync<T>(string query, object? param = null) where T : class
        {
            using var connection = _connectionFactory.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(query, param);
        }

        /// <summary>
        /// Executes a query and returns all matching records as a list.
        /// </summary>
        /// <typeparam name="T">The type to map the query results to.</typeparam>
        /// <param name="query">The SQL query string to execute.</param>
        /// <param name="param">Optional query parameters (e.g., anonymous object or dictionary).</param>
        /// <returns>
        /// A list of results mapped to <typeparamref name="T"/>.  
        /// Returns an empty list if no results are found.
        /// </returns>
        public async Task<List<T>> FindAsync<T>(string query, object? param = null) where T : class
        {
            using var connection = _connectionFactory.GetConnection();
            var results = await connection.QueryAsync<T>(query, param);
            return results.ToList();
        }

        /// <summary>
        /// Executes a non-query SQL command (e.g., INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">The SQL command to execute.</param>
        /// <param name="param">Optional query parameters (e.g., anonymous object or dictionary).</param>
        /// <returns>
        /// <c>true</c> if one or more rows were affected; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ExecuteQueryAsync(string query, object? param = null)
        {
            using var connection = _connectionFactory.GetConnection();
            var result = await connection.ExecuteAsync(query, param);
            return result > 0;
        }
    }
}
