using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sanicompras.Models;
using System.Data;

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsSubFamilyController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        private string imgHost;
        [FromHeader(Name = "token")] public string token { get; set; }

        public ProductsSubFamilyController(IConfiguration configuration)
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
        public async Task<ActionResult<PagedCollection<ProductsSubFamily>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.ProductSubfamily_GetAll);
            string offset = string.Empty;

            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }

            query = string.Format(query, offset);

            PagedCollection<ProductsSubFamily> subFamilies = new PagedCollection<ProductsSubFamily>();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        subFamilies.Data.Add(toProductSubFamily(row));
                    }

                    subFamilies.NextPage = filter.GetNextPage(Request);
                    subFamilies.PreviousPage = filter.GetPreviousPage(Request);

                    return Ok(subFamilies);
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
        public async Task<ActionResult<ProductsSubFamily>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.ProductSubfamily_GetByID);
            query = string.Format(query, id);
            ProductsSubFamily subFamily = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    subFamily = toProductSubFamily(SQL.dt.Rows[0]);
                    return Ok(subFamily);
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

        private ProductsSubFamily toProductSubFamily(DataRow row)
        {
            ProductsSubFamily oProductFamily = null;

            oProductFamily = new ProductsSubFamily();
            
            string imageName = "";
            string? imageUDF = row.Field<string?>("Image");

            if (imageUDF != null)
            {
                imageName = string.Concat(this.imgHost ?? "", imageUDF.Split("\\").Last());
            }


            oProductFamily.SubFamilyCode = row.Field<string?>("SubFamilyCode");
            oProductFamily.SubFamilyName = row.Field<string?>("SubFamilyName");
            oProductFamily.FamilyCode = row.Field<string?>("FamilyCode");
            oProductFamily.Image = imageName;


            return oProductFamily;

        }
    }
}
