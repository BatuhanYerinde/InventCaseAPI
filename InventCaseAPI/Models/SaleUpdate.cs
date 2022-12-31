using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventCaseAPI.Models
{
    public class SaleUpdate
    {
        [Range(1, int.MaxValue, ErrorMessage = "Satış Id Boş Olamaz")]
        [DefaultValue(0)]
        public int SaleId { get; set; }

        [Required(ErrorMessage = "Satış Tarihi Boş Olamaz.")]
        public DateTime Date { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Satış Miktarı Boş Olamaz")]
        [DefaultValue(0)]
        public short SaleQuantity { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "Stok Boş Olamaz")]
        [DefaultValue(0)]
        public short Stock { get; set; }
    }
}
