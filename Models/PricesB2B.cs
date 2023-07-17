namespace Sanicompras.Models
{
    public class PricesB2B
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public List<SpecialPrice> SpecialPrices { get; set; } = new List<SpecialPrice>();

        //public int Id_product { get; set; }
        //public List<Customer_group> Customer_Groups { get; set; }

    }
    public class SpecialPrice
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public Int16? ListNum { get; set; }

    }

    //public class Customer_group
    //{
    //    public int Id_group { get; set; }
    //    public string Name_group { get; set; }
    //    public double Unit_price { get; set; }
    //    public double Sale_price { get; set; }
    //}
}
