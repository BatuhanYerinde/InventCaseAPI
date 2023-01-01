using InventCaseAPI.Data.Abstract;
using InventCaseAPI.Models;
using System.Data;
using System.Data.SqlClient;

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
                using var command = new SqlCommand("SELECT sales.Id, sales.Date, sales.SalesQuantity, sales.Stock, product.ProductName, " +
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
                    command.Parameters.Add(new SqlParameter("Stock", sale.Stock));
                    insertedSaleId = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
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
            using var command = new SqlCommand($"SELECT Id FROM InventorySales WHERE Id={id};", connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                saleHistoryExist = true;
            }
            reader.Close();
            return saleHistoryExist;
        }


        public StoreProfit GetStoreProfit(int storeId)
        {
            var connection = _dbConnection.GetConnection();
            try
            {
                connection.Open();
                using var command = new SqlCommand("SELECT store.Id, store.StoreName, SUM((product.SalesPrice - product.Cost) * sales.SalesQuantity) AS Profit" +
                                                    " FROM InventorySales AS sales INNER JOIN Products AS product ON sales.ProductId = product.Id" +
                                                    $" INNER JOIN Stores AS store ON sales.StoreId = store.Id WHERE store.Id={storeId} GROUP BY store.Id, store.StoreName;", connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                var storeProfit = new StoreProfit();
                while (reader.Read())
                {
                    storeProfit.StoreId = reader.GetInt32("Id");
                    storeProfit.StoreName = reader.GetString("StoreName");
                    storeProfit.Profit = reader.GetInt32("Profit");
                    return storeProfit;
                }
                reader.Close();
                return null;
            }
            finally
            {
                connection?.Close();
            }
        }

        public StoreProfit GetMostProfitableStore()
        {
            var connection = _dbConnection.GetConnection();
            try
            {
                connection.Open();
                using var command = new SqlCommand("SELECT TOP 1 store.Id, store.StoreName, SUM((product.SalesPrice - product.Cost) * sales.SalesQuantity) profit" +
                                                    " FROM InventorySales AS sales INNER JOIN Products AS product ON sales.ProductId = product.Id" +
                                                    " INNER JOIN Stores AS store ON sales.StoreId = store.Id" +
                                                    " GROUP BY store.Id, store.StoreName ORDER BY profit DESC;", connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                var storeProfit = new StoreProfit();
                while (reader.Read())
                {
                    storeProfit.StoreId = reader.GetInt32("Id");
                    storeProfit.StoreName = reader.GetString("StoreName");
                    storeProfit.Profit = reader.GetInt32("Profit");
                    return storeProfit;
                }
                reader.Close();
                return null;
            }
            finally
            {
                connection?.Close();
            }
        }

        public BestSellerProduct GetBestSellerProduct()
        {
            var connection = _dbConnection.GetConnection();
            try
            {
                connection.Open();
                using var command = new SqlCommand("SELECT TOP 1 product.Id, product.ProductName, SUM(sales.SalesQuantity) AS SalesQuantity" +
                                                    " FROM InventorySales AS sales INNER JOIN Products AS product ON sales.ProductId = product.Id" +
                                                    " INNER JOIN Stores AS store ON sales.StoreId = store.Id" +
                                                    " GROUP BY product.Id, product.ProductName ORDER BY SalesQuantity DESC;", connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                var bestSellerProduct = new BestSellerProduct();
                while (reader.Read())
                {
                    bestSellerProduct.Id = reader.GetInt32("Id");
                    bestSellerProduct.Name = reader.GetString("ProductName");
                    bestSellerProduct.SalesQuantity = reader.GetInt32("SalesQuantity");
                    return bestSellerProduct;
                }
                reader.Close();
                return null;
            }
            finally
            {
                connection?.Close();
            }
        }
    }
}
