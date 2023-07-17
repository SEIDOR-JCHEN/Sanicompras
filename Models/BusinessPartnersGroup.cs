using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Sanicompras.Models
{
    
    public class BusinessPartnersGroup
    {
        public Int16 GroupCode { get; set; }
        public string? GroupName { get; set; }
        public Int16? PriceList { get; set; }
    }

}


