using InventCaseAPI.Data.Abstract;
using InventCaseAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace InventCaseAPI.Data.Concrete
{
    public class SaleDAL : ISaleDAL
    {
        private readonly DbConnection _dbConnection;
        public SaleDAL(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public List<Sale> GetSalesHistory()
        {
            var connection = _dbConnection.GetConnection();
            var salesHistory = new List<Sale>();
            try
            {
                SqlCommand command = new SqlCommand("SELECT sales.Id, sales.Date, sales.SalesQuantity, sales.Stock, product.ProductName, " +
                                                    "product.Cost, product.SalesPrice, store.StoreName FROM InventorySales AS sales " +
                                                    "INNER JOIN Products AS product ON sales.ProductId = product.Id " +
                                                    "INNER JOIN Stores AS store ON sales.StoreId = store.Id;", connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    salesHistory.Add(new Sale
                    {
                        SaleId = reader.GetInt32("Id"),
                        Date = reader.GetDateTime("Date").Date,
                        SaleQuantity = reader.GetInt16("SalesQuantity"),
                        Stock = reader.GetInt16("Stock"),
                        ProductName = reader.GetString("ProductName"),
                        Cost = reader.GetInt32("Cost"),
                        SalesPrice = reader.GetInt32("SalesPrice"),
                        StoreName = reader.GetString("StoreName")
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSalesHistory Error : {ex.ToString()}");
                throw ex;
            }
            finally
            {
                connection?.Close();
            }
            return salesHistory;
        }

        public int AddSaleHistory(SalePost sale)
        {
            int insertedSaleId = 0;
            var connection = _dbConnection.GetConnection();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(
                "INSERT INTO InventorySales OUTPUT Inserted.Id VALUES(@ProductId, @StoreId, @Date, @SalesQuantity, @Stock);", connection, transaction))
                {
                    command.Parameters.Add(new SqlParameter("ProductId", sale.ProductId));
                    command.Parameters.Add(new SqlParameter("StoreId", sale.StoreId));
                    command.Parameters.Add(new SqlParameter("Date", sale.Date.Date));
                    command.Parameters.Add(new SqlParameter("SalesQuantity", sale.SaleQuantity));
                    command.Parameters.Add(new SqlParameter("Stock", sale.Stock - sale.SaleQuantity));
                    insertedSaleId = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                System.Diagnostics.Debug.WriteLine($"AddSaleHistory Error : {ex.ToString()}");
                throw ex;
            }
            finally
            {
                connection?.Close();
            }
            return insertedSaleId;
        }

        public int UpdateSaleHistory(SaleUpdate saleUpdate)
        {
            int updatedSaleId = 0;
            var connection = _dbConnection.GetConnection();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                if (CheckSaleHistoryExist(connection, saleUpdate.SaleId))
                {
                    transaction = connection.BeginTransaction();
                    using (SqlCommand command = new SqlCommand(
                    "UPDATE InventorySales SET Date=@Date, SalesQuantity=@SalesQuantity, Stock=@Stock OUTPUT INSERTED.Id WHERE Id=@SaleId;", connection, transaction))
                    {
                        command.Parameters.Add(new SqlParameter("SaleId", saleUpdate.SaleId));
                        command.Parameters.Add(new SqlParameter("Date", saleUpdate.Date));
                        command.Parameters.Add(new SqlParameter("SalesQuantity", saleUpdate.SaleQuantity));
                        command.Parameters.Add(new SqlParameter("Stock", saleUpdate.Stock));
                        updatedSaleId = (int)command.ExecuteScalar();

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                System.Diagnostics.Debug.WriteLine($"UpdateSaleHistory Error : {ex.ToString()}");
                throw ex;
            }
            finally
            {
                connection?.Close();
            }
            return updatedSaleId;
        }

        public int DeleteSaleHistory(int id)
        {
            int deletedSaleId = 0;
            var connection = _dbConnection.GetConnection();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                if (CheckSaleHistoryExist(connection, id))
                {
                    transaction = connection.BeginTransaction();
                    using (SqlCommand command = new SqlCommand(
                    "DELETE FROM InventorySales OUTPUT DELETED.Id WHERE Id=@SaleId;", connection, transaction))
                    {
                        command.Parameters.Add(new SqlParameter("SaleId", id));
                        deletedSaleId = (int)command.ExecuteScalar();

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                System.Diagnostics.Debug.WriteLine($"DeleteSaleHistory Error : {ex.ToString()}");
                throw ex;
            }
            finally
            {
                connection?.Close();
            }
            return deletedSaleId;
        }

        private bool CheckSaleHistoryExist(SqlConnection connection, int id)
        {
            bool saleHistoryExist = false;
            try
            {
                SqlCommand command = new SqlCommand($"SELECT Id FROM InventorySales WHERE Id={id};", connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    saleHistoryExist = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckSaleHistoryExist Error : {ex.ToString()}");
                throw ex;
            }
            return saleHistoryExist;
        }
    }
}
