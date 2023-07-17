namespace Sanicompras.Models
{
    //public class Prices
    //{
    //    public Int16 ListNum { get; set; }

    //    public string ListName { get; set; }

    //    public List<PriceItem> Items { get; set; } = new List<PriceItem>();

    //    //public int Id_product { get; set; }
    //    //public List<Shop> Shops { get; set; }
    //}

    //public class PriceItem
    //{
    //    public string ItemCode { get; set; }
    //    public string ItemName { get; set; }
    //    public DateTime CreateDateTime { get; set; }
    //    public DateTime UpdateDateTime { get; set; }
    //    public decimal Price { get; set; }
    //    public string Currency { get; set; }
    //    public Int16 BASE_NUM { get; set; }
    //    public decimal Factor { get; set; }

    //}

   
    public class Prices
    {
        //public int Id_product { get; set; }
        //public List<Shop> Shops { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }

        public List<PriceList> PriceLists { get; set; } = new List<PriceList>();

    }
    //public class PriceList
    //{
    //    public Int16 ListNum { get; set; }
    //    public string ListName { get; set; }
    //    public decimal Price { get; set; }
    //    public string Currency { get; set; }
    //    public Int16 BASE_NUM { get; set; }
    //    public decimal Factor { get; set; }
        
    //}

    //public class Shop
    //{
    //    public int Id_shop { get; set; }
    //    public double Unit_price { get; set; }
    //    public double Sale_price { get; set; }
    //    public double Whosale_price { get; set; }

    //}
}
