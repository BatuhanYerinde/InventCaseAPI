using InventCaseAPI.Models;
using System.Data.SqlClient;

namespace InventCaseAPI.Data.Abstract
{
    public interface ISaleDAL
    {
        List<Sale> GetSalesHistory();

        int AddSaleHistory(SalePost salePost);

        int UpdateSaleHistory(SaleUpdate saleUpdate);

        int DeleteSaleHistory(int id);

        StoreProfit GetStoreProfit(int storeId);

        StoreProfit GetMostProfitableStore();

        BestSellerProduct GetBestSellerProduct();
    }
}
