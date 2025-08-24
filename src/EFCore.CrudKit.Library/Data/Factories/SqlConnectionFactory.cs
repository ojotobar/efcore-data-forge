using EFCore.CrudKit.Library.Data.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EFCore.CrudKit.Library.Data.Factories
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration, string subSectionName)
        {
            var connectionString = configuration.GetConnectionString(subSectionName) ?? 
                throw new ArgumentNullException("Connection String is null");
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
