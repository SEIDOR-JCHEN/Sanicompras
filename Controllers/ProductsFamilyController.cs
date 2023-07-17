using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sanicompras.Models;
using System.Data;

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsFamilyController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        private string imgHost;
        [FromHeader(Name = "token")] public string token { get; set; }

        public ProductsFamilyController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);

            this.Inicialize();
        }

        private void Inicialize()
        {
            string paramTableQuery = Querys.getQuery(SQLFiles.GetProductParams);
            SQL.DoQuery(paramTableQuery);

            if (SQL.count > 0)
            {
                foreach (DataRow row in SQL.dt.Rows)
                {
                    switch (row.Field<string>("Code"))
                    {
                        case "SEI_PS_ImgHost":
                            {
                                this.imgHost = row.Field<string>("Name") ?? "";
                                break;
                            }
                    }
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<ProductsFamily>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.ProductFamily_GetAll);
            string offset = string.Empty;

            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }

            query = string.Format(query, offset);

            PagedCollection<ProductsFamily> productFamilies = new PagedCollection<ProductsFamily>();
    
            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach(DataRow row in SQL.dt.Rows)
                    {
                        productFamilies.Data.Add(toProductFamily(row));
                    }
                    
                    productFamilies.NextPage = filter.GetNextPage(Request);
                    productFamilies.PreviousPage = filter.GetPreviousPage(Request);
              
                    return Ok(productFamilies);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

      

        [HttpGet("GetByDate")]
        public async Task<ActionResult<PagedCollection<ProductsFamily>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.ProductFamily_GetByDate);
            string offset = string.Empty;

            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<ProductsFamily> productFamilies = new PagedCollection<ProductsFamily>();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        productFamilies.Data.Add(toProductFamily(row));
                    }

                    productFamilies.NextPage = filter.GetNextPage(Request);
                    productFamilies.PreviousPage = filter.GetPreviousPage(Request);
              
                    return Ok(productFamilies);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsFamily>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.ProductFamily_GetByID);
            query = string.Format(query, id);
            ProductsFamily productFamily = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    productFamily = toProductFamily(SQL.dt.Rows[0]);
                    return Ok(productFamily);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ProductsFamily toProductFamily(DataRow row)
        {
            ProductsFamily oProductFamily = null;

            string imageName = "";
            string? imageUDF = row.Field<string?>("Image");

            if(imageUDF != null)
            {
                imageName = string.Concat(this.imgHost ?? "", imageUDF.Split("\\").Last());
            }
            
            oProductFamily = new ProductsFamily();
            oProductFamily.FamilyCode = row.Field<Int16?>("FamilyCode");
            oProductFamily.FamilyName = row.Field<string?>("FamilyName");
            oProductFamily.Image = imageName;
            oProductFamily.ToShop = row.Field<string?>("ToShop");
            oProductFamily.GrupoClientes = row.Field<string?>("GrupoClientes");
            oProductFamily.PriceList = row.Field<Int32?>("PriceList");
            oProductFamily.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
            oProductFamily.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");
            

            return oProductFamily;

        }


    }
}
