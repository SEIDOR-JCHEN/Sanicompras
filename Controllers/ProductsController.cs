using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR.Protocol;
using RestSharp.Extensions;
using Sanicompras.Models;
using System.Configuration;
using System.Data;

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [FromHeader(Name = "token")]
        public string token { get; set; }

        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        private string imgHost;
        private string imgFolder;

        public ProductsController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);

            this.inicialize();
            
        }

        private void inicialize()
        {
            string paramTableQuery = Querys.getQuery(SQLFiles.GetProductParams);
            SQL.DoQuery(paramTableQuery);

            if(SQL.count > 0)
            {
                foreach(DataRow row in SQL.dt.Rows)
                {
                    switch (row.Field<string>("Code"))
                    {
                        case "SEI_PS_ImgHost":
                            {
                                this.imgHost = row.Field<string>("Name") ?? "";
                                break;
                            }
                        case "SEI_PS_ImgFolder":
                            {
                                this.imgFolder = row.Field<string>("Name") ?? "";
                                break;
                            }
                    }
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<Products>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }
            
            string query = Querys.getQuery(SQLFiles.Products_GetAll);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            query = string.Format(query, offset);

            PagedCollection<Products> products = new PagedCollection<Products>();
            Products currentProduct = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    //string currentItem = "";
                    DataTable items = SQL.dt;

                    foreach (DataRow itemsRow in items.Rows)
                    {
                        
                        currentProduct = new Products();
                        FillProduct(currentProduct, itemsRow);
                        products.Data.Add(currentProduct);

                    }
                    products.NextPage = filter.GetNextPage(Request);
                    products.PreviousPage = filter.GetPreviousPage(Request);

                    return Ok(products);

                } else
                {
                    return NotFound(products);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        [HttpGet("GetByDate")]
        public async Task<ActionResult<PagedCollection<Products>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Products_GetByDate);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<Products> products = new PagedCollection<Products>();
            Products currentProduct = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    //string currentItem = "";
                    DataTable items = SQL.dt;

                    foreach (DataRow itemsRow in items.Rows)
                    {
                        currentProduct = new Products();
                        FillProduct(currentProduct, itemsRow);
                        products.Data.Add(currentProduct);
                    }

                    products.NextPage = filter.GetNextPage(Request);
                    products.PreviousPage = filter.GetPreviousPage(Request);
                    return Ok(products);
                }
                else
                {
                    return NotFound(products);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Products_GetByID);
            query = string.Format(query, id);

            Products product = new Products();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {

                    FillProduct(product,SQL.dt.Rows[0]);

                    return Ok(product);
                }
                else
                {
                    return NotFound(product);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //private void AddAlternative(DataRow row, Products product)
        //{
        //    Alternative alternative = new Alternative();

        //    alternative.ItemCode = row.Field<string?>("AlternativeItemCode");
        //    alternative.ItemName = row.Field<string?>("AlternativeItemName");

        //    product.alternatives.Add(alternative);

        //}

        
        private void FillProduct(Products currentProduct, DataRow currentRow)
        {

            currentProduct.ItemCode = currentRow.Field<string?>("ItemCode");
            currentProduct.Title = currentRow.Field<string?>("Title");
            currentProduct.ShortDesc = currentRow.Field<string?>("ShortDesc");
            currentProduct.LongDesc = currentRow.Field<string?>("LongDesc");
            currentProduct.SWeight = currentRow.Field<decimal?>("Weight");
            currentProduct.FamilyCode = currentRow.Field<Int16?>("FamilyCode");
            currentProduct.FamilyName = currentRow.Field<string?>("FamilyName");
            currentProduct.SubFamilyCode = currentRow.Field<string?>("SubFamilyCode");
            currentProduct.SubFamilyName = currentRow.Field<string?>("SubFamilyName");
            currentProduct.Marca = currentRow.Field<string?>("Marca");
            currentProduct.Active = currentRow.Field<string?>("Active");

            ItemAttribute size = new ItemAttribute();
            size.name = "Size";
            size.value = currentRow.Field<string?>("Size");
            currentProduct.Attributes.Add(size);

            ItemAttribute color = new ItemAttribute();
            color.name = "Color";
            color.value = currentRow.Field<string?>("Color");
            currentProduct.Attributes.Add(color);
   
            currentProduct.CreateDateTime = currentRow.Field<DateTime?>("CreateDateTime");
            currentProduct.UpdateDateTime = currentRow.Field<DateTime?>("UpdateDateTime");

            string query = Querys.getQuery(SQLFiles.GetPriceLists);

            query = string.Format(query, currentRow.Field<string?>("ItemCode"));
            SQL.DoQuery(query);
            DataTable PriceLists = SQL.dt;

            foreach (DataRow priceListsRow in PriceLists.Rows)
            {
                PriceList pl = new PriceList();

                pl.ListNum = priceListsRow.Field<Int16?>("ListNum");
                pl.ListName = priceListsRow.Field<string?>("ListName");
                pl.Price = priceListsRow.Field<decimal?>("Price");
                pl.Currency = priceListsRow.Field<string?>("Currency");

                currentProduct.PriceLists.Add(pl);
            }

            query = Querys.getQuery(SQLFiles.GetPriceB2B);
            query = string.Format(query, currentRow.Field<string?>("ItemCode"));
            SQL.DoQuery(query);
            DataTable PriceB2Bs = SQL.dt;

            foreach (DataRow PriceB2BRow in PriceB2Bs.Rows)
            {
                PriceB2B SP_Price = new PriceB2B();

                SP_Price.CardCode = PriceB2BRow.Field<string?>("CardCode");
                SP_Price.CardName = PriceB2BRow.Field<string?>("CardName");
                SP_Price.Price = PriceB2BRow.Field<decimal?>("Price");

                currentProduct.PriceB2B.Add(SP_Price);
            }

            query = Querys.getQuery(SQLFiles.GetAlternatives);
            query = string.Format(query, currentRow.Field<string?>("ItemCode"));
            SQL.DoQuery(query);
            DataTable alternatives = SQL.dt;

            foreach (DataRow item in alternatives.Rows)
            {
                Variant variant = new Variant();

                variant.ItemCode = item.Field<string?>("ItemCode");
                variant.Title = item.Field<string?>("Title");


                ItemAttribute vSize = new ItemAttribute();
                vSize.name = "Size";
                vSize.value = item.Field<string?>("Size");
                variant.Attributes.Add(vSize);

                ItemAttribute vColor = new ItemAttribute();
                vColor.name = "Color";
                vColor.value = item.Field<string?>("Color");
                variant.Attributes.Add(vColor);

                currentProduct.Variants.Add(variant);
            }


            query = Querys.getQuery(SQLFiles.GetProductProps);
            SQL.DoQuery(query);

            List<Propiedad> listProp = new List<Propiedad>();
            DataTable props = SQL.dt;

            foreach(DataRow prop in props.Rows)
            {
                Propiedad propObj = new Propiedad();
                propObj.Id = prop.Field<Int16>("ItmsTypCod");
                propObj.Name = prop.Field<string?>("ItmsGrpNam");

                listProp.Add(propObj);
            }

            for(int i = 1; i <= 64; i++)
            {
                string checkedProp = currentRow.Field<string?>(string.Format("QryGroup{0}", i));
                if (checkedProp == "Y")
                {
                    Propiedad prop = listProp.Find(p => p.Id == i);
                    if(prop != null)
                    {
                        currentProduct.Properties.Add(prop);
                    }
                    
                }
               
            }

            string fileName = currentRow.Field<string?>("ItemCode");
            string suffix = "";
            int imgCount = 0;

            while (true)
            {
                Image img = new Image();
                
                if(imgCount > 0)
                {
                    suffix = "_" + imgCount;
                }

                string? imgUrl = getImageIfExist(fileName + suffix);
                
                if(imgUrl != null)
                {
                    img.url = imgUrl;
                    currentProduct.Images.Add(img);
                    imgCount++;
                }
                else
                {
                    break;
                }


            }
          
        }

        private string? getImageIfExist(string fileName)
        {
            string extension = ".jpg";
            string filePath = Path.Combine(this.imgFolder, fileName + extension);
            DirectoryInfo directoryInfo = new DirectoryInfo(this.imgFolder);
            FileInfo[] files = directoryInfo.GetFiles(fileName + extension, SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                string actualFileName = files[0].Name;
                return this.imgHost + actualFileName;
            }
            else
            {
                extension = ".png";
                files = directoryInfo.GetFiles(fileName + extension, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    string actualFileName = files[0].Name;
                    return this.imgHost + actualFileName;
                }
                else
                {
                    return null;
                }
            }
            
        }


    }
}
