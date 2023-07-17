using Sanicompras.Models.Generals;

namespace Sanicompras.Models
{
    public class Stocks
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        //public string HashCode { get; set; }
        //public string Ean13 { get; set; }
        public List<StockInWhareHouse> StockInWhareHouses { get; set; } = new List<StockInWhareHouse>();

    }

    public class StockInWhareHouse
    {
        public string? WhsCode { get; set; }
        public string? WhsName { get; set; }
        public decimal? Quantity { get; set; }
    }

}
