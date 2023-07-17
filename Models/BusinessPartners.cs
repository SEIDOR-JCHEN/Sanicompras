using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Sanicompras.Models
{
    
    public class BusinessPartners
    {

        public string? CardCode { get; set; }

        public string? CardName { get; set; }

        [JsonProperty(PropertyName = "U_SEI_PS_Nombre")]
        public string? FirstName { get; set; }

        [JsonProperty(PropertyName = "U_SEI_PS_Apellido")]
        public string? LastName { get; set; }

        public string? CardType { get; set; }

        public Int16? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public string? FederalTaxID { get; set; }

        public string? EmailAddress { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }
        public int? Series { get; set; }

        [JsonProperty(PropertyName = "PriceListNum")]
        public Int16? ListNum { get; set; }

        public string? ListName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }

        [JsonProperty(PropertyName = "BPAddresses")]
        public List<Direction>? Addresses { get; set; } = new List<Direction>();
       
    }


    public class Direction
    {
        public string? AddressCode { get; set; }
        [JsonProperty(PropertyName = "BPCode")]
        public string? CardCode { get; set; }

        [JsonProperty(PropertyName = "RowNum")]
        public int? Number { get; set; }
        public string? AddressType { get; set; }
        public string? AddressName { get; set; }
        public string? Street { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Country { get; set; }

        [JsonProperty(PropertyName = "BuildingFloorRoom")]
        public string? Phone { get; set; }
    }
}


