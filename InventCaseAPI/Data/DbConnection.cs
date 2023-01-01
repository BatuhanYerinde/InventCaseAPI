using InventCaseAPI.Constants.Configurations;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace InventCaseAPI.Data
{
    public class DbConnection
    {
        private readonly ConnectionStrings _connectionStrings;
        public DbConnection(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }
        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionStrings.Default);
            return connection;
        }
    }
}
