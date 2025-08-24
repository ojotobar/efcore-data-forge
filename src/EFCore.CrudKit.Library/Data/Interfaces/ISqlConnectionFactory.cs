using Microsoft.Data.SqlClient;

namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface ISqlConnectionFactory
    {
        SqlConnection GetConnection();
    }
}
