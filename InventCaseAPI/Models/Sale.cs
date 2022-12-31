namespace InventCaseAPI.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public string ProductName { get; set; }

        public int Cost { get; set; }

        public int SalesPrice { get; set; }

        public string StoreName { get; set; }

        public DateTime Date { get; set; }

        public short SaleQuantity { get; set; }

        public short Stock { get; set; }


    }
}
