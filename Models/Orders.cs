namespace Sanicompras.Models
{
    public class Orders
    {
        //public string? SanicomprasID { get; set; }
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public decimal? DocTotal { get; set; }
        
        public BusinessPartners? Custumer { get; set; } = new BusinessPartners();
        public List<DocumentLine>? DocumentLines { get; set; } = new List<DocumentLine>();
    }

}
