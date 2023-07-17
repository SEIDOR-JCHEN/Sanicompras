using Sanicompras.Models.Generals;

namespace Sanicompras.Models
{
    public class ProductsFamily
    {
        public Int16? FamilyCode { get; set; }
        public string? FamilyName { get; set; }
        public string? Image { get; set; }
        public string? ToShop { get; set; }
        public string? GrupoClientes { get; set; }
        public Int32? PriceList { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }

    }


}
