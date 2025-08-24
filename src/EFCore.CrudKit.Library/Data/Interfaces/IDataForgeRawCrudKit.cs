
namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface IDataForgeRawCrudKit
    {
        Task<bool> ExecuteQueryAsync(string query, object? param = null);
        Task<List<T>> FindAsync<T>(string query, object? param = null) where T : class;
        Task<T?> FindOneAsync<T>(string query, object? param = null) where T : class;
    }
}
