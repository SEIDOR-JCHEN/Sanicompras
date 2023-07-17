using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sanicompras.Models;
using System.Data;

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersGroupsController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;

        [FromHeader(Name = "token")] public string token { get; set; }
        public CustomersGroupsController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);
            Inicialize();
        }

        private void Inicialize()
        {
            
        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<BusinessPartnersGroup>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.CustomersGroup_GetAll);
            string offset = string.Empty;

            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }

            query = string.Format(query, offset);

            PagedCollection<BusinessPartnersGroup> bpGroups = new PagedCollection<BusinessPartnersGroup>();
    
            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach(DataRow row in SQL.dt.Rows)
                    {
                        bpGroups.Data.Add(toBpGroup(row));
                    }

                    bpGroups.NextPage = filter.GetNextPage(Request);
                    bpGroups.PreviousPage = filter.GetPreviousPage(Request);
              
                    return Ok(bpGroups);
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
        public async Task<ActionResult<BusinessPartnersGroup>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.CustomersGroup_GetByID);
            query = string.Format(query, id);
            BusinessPartnersGroup bpGroup = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    bpGroup = toBpGroup(SQL.dt.Rows[0]);
                    return Ok(bpGroup);
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


        private BusinessPartnersGroup toBpGroup(DataRow row)
        {
            BusinessPartnersGroup bpGroup = null;

  
            bpGroup = new BusinessPartnersGroup();
            bpGroup.GroupCode = row.Field<Int16>("GroupCode");
            bpGroup.GroupName = row.Field<string?>("GroupName");
            bpGroup.PriceList = row.Field<Int16?>("PriceList");
  
            return bpGroup;
        }


    }
}
