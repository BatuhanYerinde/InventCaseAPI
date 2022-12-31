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
            try
            {
                var connection = new SqlConnection(_connectionStrings.Default);
                return connection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetConnection Error : {ex.ToString()}");
                throw new Exception($"GetConnection Error : {ex.ToString()}");
            }
        }
    }
}
