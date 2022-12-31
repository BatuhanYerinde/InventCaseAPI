using InventCaseAPI.Models;

namespace InventCaseAPI.Data.Abstract
{
    public interface ISaleDAL
    {
        List<Sale> GetSalesHistory();

        int AddSaleHistory(SalePost salePost);

        int UpdateSaleHistory(SaleUpdate saleUpdate);

        int DeleteSaleHistory(int id);
    }
}
