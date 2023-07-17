using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Sanicompras.Models.Generals;
using System.Security.Permissions;

namespace Sanicompras.Models
{
    public class Products
    {
        public string? ItemCode { get; set; }
        public string? Title { get; set; }
        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; } 
        public string? Active { get; set; }
        public decimal? SWeight { get; set; }
        public string? SubFamilyCode { get; set; }
        public string? SubFamilyName { get; set; }
        public Int16? FamilyCode { get; set; }
        public string? FamilyName { get; set; }
        public string? Marca { get; set; }
        public List<ItemAttribute?> Attributes { get; set; } = new List<ItemAttribute>();
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public List<PriceList>? PriceLists { get; set; } = new List<PriceList>();
        public List<PriceB2B>? PriceB2B { get; set; } = new List<PriceB2B>();
        public List<Propiedad>? Properties { get; set; } = new List<Propiedad>();
        public List<Variant>? Variants { get; set; } = new List<Variant>();
        public List<Image> Images { get; set; } = new List<Image>();

        //public string Marca { get; set; }

        //public string Hashcode { get; set; }

        //List of category
        //public List<Alternative>? Alternatives { get; set; } = new List<Alternative>();


    }

    public class Propiedad
    {
        public Int16 Id { get; set; }
        public string Name { get; set; }

    }

    public class PriceList
    {
        public Int16? ListNum { get; set; }
        public string? ListName { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
    }

    public class PriceB2B
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public decimal? Price { get; set; }
    }

    public class Variant
    {
        public string? ItemCode { get; set; }

        [JsonProperty(PropertyName = "ItemName")]
        public string? Title { get; set; }
        public List<ItemAttribute> Attributes { get; set; } = new List<ItemAttribute>();

    }

    public class Image
    {
        public string? url { get; set; }

    }

    public class ItemAttribute
    {
        public string? name { get; set; }
        public string? value { get; set; }
    }
    //    public class Image
    //    {
    //        public int Number { get; set; }
    //        public string ImageLink { get; set; }
    //        public string Hash { get; set; }
    //    }

    //    public class ProductCategory
    //    {
    //        public int id_category { get; set; }
    //    }

    //    public class Feature
    //    {
    //        public int Id_feature_group { get; set; }
    //        public string name_feature_group { get; set; }
    //        public string value_feature { get; set; }
    //    }


}
