using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventCaseAPI.Models
{
    public class SalePost
    {
        //Fluent validation can be used instead of data anatotion.

        [Range(1, int.MaxValue, ErrorMessage = "Ürün Boş Olamaz")]
        [DefaultValue(0)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mağaza Boş Olamaz")]
        [DefaultValue(0)]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Satış Tarihi Boş Olamaz.")]
        public DateTime Date { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Satış Miktarı Doğru Girilmelidir")]
        [DefaultValue(0)]
        public short SaleQuantity { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "Stok Miktarı Doğru Girilmelidir")]
        [DefaultValue(0)]
        public short Stock { get; set; }
    }
}
