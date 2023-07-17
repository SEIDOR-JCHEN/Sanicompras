namespace Sanicompras.Models
{
    public class Documents
    {
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public decimal? DocTotal { get; set; }
        public string? DocStatus { get; set; }
        public string? Canceled { get; set; }
        public string? CardCode { get; set; }
        public List<DocumentLine> DocumentLines { get; set; } = new List<DocumentLine>();
    }

    public class DocumentLine
    {
        public string? ItemCode { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? Quantity { get; set; }
        public string? VatGroup { get; set; }
        public decimal? LineTotal { get; set; }

    }
}
