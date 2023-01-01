using InventCaseAPI.Data.Abstract;
using InventCaseAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventCaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanySalesController : ControllerBase
    {
        private readonly ISaleDAL _saleDal;
        public CompanySalesController(ISaleDAL saleDal)
        {
            _saleDal = saleDal;
        }

        [HttpGet]
        public ActionResult<List<Sale>> GetSales()
        {
            var salesHistory = _saleDal.GetSalesHistory();
            if (salesHistory == null) 
                return NotFound();
            return Ok(salesHistory);
        }

        [HttpPost]
        public ActionResult<int> AddSaleHistory([FromBody] SalePost salePost)
        {
            int Id = _saleDal.AddSaleHistory(salePost);
            if (Id > 0) 
                return Ok(Id);
            return BadRequest();
        }

        [HttpPut]
        public ActionResult<int> UpdateSaleHistory([FromBody] SaleUpdate saleUpdate)
        {
            int Id = _saleDal.UpdateSaleHistory(saleUpdate);
            if (Id > 0)
                return Ok(Id);
            return BadRequest();
        }

        [HttpDelete]
        public ActionResult<int> DeleteSaleHistory([FromQuery] int saleId)
        {
            int Id = _saleDal.DeleteSaleHistory(saleId);
            if (Id > 0)
                return Ok(Id);
            return BadRequest();
        }

        [HttpGet("store-profit")]
        public ActionResult<StoreProfit> GetStoreProfit([FromQuery] int storeId)
        {
            var storeProfit = _saleDal.GetStoreProfit(storeId);
            if (storeProfit == null)
                return BadRequest();
            return Ok(storeProfit);
        }

        [HttpGet("most-profitable-store")]
        public ActionResult<StoreProfit> GetMostProfitableStore()
        {
            var mostProfitableStore = _saleDal.GetMostProfitableStore();
            if (mostProfitableStore == null)
                return NotFound();
            return Ok(mostProfitableStore);
        }

        [HttpGet("best-seller-product")]
        public ActionResult<BestSellerProduct> GetBestSellerProduct()
        {
            var bestSellerProduct = _saleDal.GetBestSellerProduct();
            if (bestSellerProduct == null)
                return NotFound();
            return Ok(bestSellerProduct);
        }
    }
}
