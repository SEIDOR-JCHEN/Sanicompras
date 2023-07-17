using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using RestSharp;
using Sanicompras.Classes;
using Sanicompras.Models;
using System.Data;
using System.Net;
using log4net.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        [FromHeader(Name = "token")] public string token { get; set; }        
        private ServiceLayer SAP = new ServiceLayer();
        private string SessionId = string.Empty;
      
        public CustomersController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);
        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<BusinessPartners>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            PagedCollection<BusinessPartners> customers = new PagedCollection<BusinessPartners>();
            BusinessPartners currentCustomer = null;
            string CurrentCardCode = null;

            string query = Querys.getQuery(SQLFiles.Customers_GetAll);
            string offset = string.Empty;
            if(filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page-1) * filter.Limit;
                offset = $"ORDER BY t1.\"CardCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            query = string.Format(query, offset);

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {                   
                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        if (row.Field<string>("CardCode") != CurrentCardCode)
                        {
                            if (CurrentCardCode != null)
                            {
                                customers.Data.Add(currentCustomer);
                            }

                            currentCustomer = new BusinessPartners();
                            currentCustomer.CardCode = row.Field<string?>("CardCode");
                            currentCustomer.CardName = row.Field<string?>("CardName");
                            currentCustomer.FirstName = row.Field<string?>("Nombre");
                            currentCustomer.LastName = row.Field<string?>("Apellido");
                            currentCustomer.CardType = row.Field<string?>("CardType");
                            currentCustomer.Phone1 = row.Field<string?>("Phone1");
                            currentCustomer.Phone2 = row.Field<string?>("Phone2");
                            currentCustomer.GroupCode = row.Field<Int16?>("GroupCode");
                            currentCustomer.GroupName = row.Field<string?>("GroupName");
                            currentCustomer.FederalTaxID = row.Field<string?>("LicTradNum");
                            currentCustomer.EmailAddress = row.Field<string?>("E_Mail");
                            currentCustomer.ListNum = row.Field<Int16?>("ListNum");
                            currentCustomer.ListName = row.Field<string?>("ListName");
                            currentCustomer.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentCustomer.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");

                            CurrentCardCode = row.Field<string?>("CardCode");

                            AddDirection(row, currentCustomer);
 
                        }
                        else
                        {
                            AddDirection(row, currentCustomer);
                        }
                    }

                    customers.Data.Add(currentCustomer);
                    customers.NextPage = filter.GetNextPage(Request);
                    customers.PreviousPage = filter.GetPreviousPage(Request);
      
                    return Ok(customers);
                }
                else
                {
                    return NotFound(customers);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("GetByDate")]
        public async Task<ActionResult<PagedCollection<BusinessPartners>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Customers_GetByDate);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t.\"CardCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<BusinessPartners> customers = new PagedCollection<BusinessPartners>();
            BusinessPartners currentCustomer = null;
            string CurrentCardCode = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    string currentItem = "";

                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        if (row.Field<string>("CardCode") != CurrentCardCode)
                        {
                            if (CurrentCardCode != null)
                            {
                                customers.Data.Add(currentCustomer);
                            }

                            currentCustomer = new BusinessPartners();
                            currentCustomer.CardCode = row.Field<string?>("CardCode");
                            currentCustomer.CardName = row.Field<string?>("CardName");
                            currentCustomer.FirstName = row.Field<string?>("Nombre");
                            currentCustomer.LastName = row.Field<string?>("Apellido");
                            currentCustomer.CardType = row.Field<string?>("CardType");
                            currentCustomer.Phone1 = row.Field<string?>("Phone1");
                            currentCustomer.Phone2 = row.Field<string?>("Phone2");
                            currentCustomer.GroupCode = row.Field<Int16?>("GroupCode");
                            currentCustomer.GroupName = row.Field<string?>("GroupName");
                            currentCustomer.FederalTaxID = row.Field<string?>("LicTradNum");
                            currentCustomer.EmailAddress = row.Field<string?>("E_Mail");
                            currentCustomer.ListNum = row.Field<Int16?>("ListNum");
                            currentCustomer.ListName = row.Field<string?>("ListName");
                            currentCustomer.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentCustomer.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");

                            CurrentCardCode = row.Field<string>("CardCode");

                            AddDirection(row, currentCustomer);

                        }
                        else
                        {
                            AddDirection(row, currentCustomer);
                        }

                    }
                    customers.Data.Add(currentCustomer);
                    customers.NextPage = filter.GetNextPage(Request);
                    customers.PreviousPage = filter.GetPreviousPage(Request);

                    return Ok(customers);
                }
                else
                {
                    return NotFound(customers);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessPartners>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            BusinessPartners customer = new BusinessPartners();
            string query = Querys.getQuery(SQLFiles.Customers_GetByID);
            query = string.Format(query, id);

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    DataRow row = SQL.dt.Rows[0];
                    customer.CardCode = row.Field<string?>("CardCode");
                    customer.CardName = row.Field<string?>("CardName");
                    customer.FirstName = row.Field<string?>("Nombre");
                    customer.LastName = row.Field<string?>("Apellido");
                    customer.CardType = row.Field<string?>("CardType");
                    customer.Phone1 = row.Field<string?>("Phone1");
                    customer.Phone2 = row.Field<string?>("Phone2");
                    customer.GroupCode = row.Field<Int16?>("GroupCode");
                    customer.GroupName = row.Field<string?>("GroupName");
                    customer.FederalTaxID = row.Field<string?>("LicTradNum");
                    customer.EmailAddress = row.Field<string?>("E_Mail");
                    customer.ListNum = row.Field<Int16?>("ListNum");
                    customer.ListName = row.Field<string?>("ListName");
                    customer.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                    customer.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");

                    foreach (DataRow directions in SQL.dt.Rows)
                    {
                        AddDirection(directions, customer);
                    }

                    return Ok(customer);
                }
                else
                {
                    return NotFound(customer);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST api/<CustomerController>
        [HttpPost]
        public ActionResult Post([FromBody] BusinessPartners customer)
        {
            string responseContent = string.Empty;
            string jsonBody = string.Empty;
            HttpStatusCode? statusCode = null;

            ActionResult response = null;
            if (!CheckAndLogin(ref response))
            {
                return response;
            };


            if (customer.Phone1 == null || customer.EmailAddress == null)
            {
                return BadRequest("Phone or Email can not be empty!");
            }


            try
            {
                //Format Model
                customer.CardCode = null;
                customer.ListName = null;
                customer.GroupName = null;
                customer.Series = int.Parse(Configuration["CustomerSeries"]);
                customer.CardType = "C";
                customer.CreateDateTime = null;
                customer.UpdateDateTime = null;

                Int16? psListNum = null;

                if(customer.ListNum != null)
                {
                    psListNum = customer.ListNum;

                    string query = "SELECT \"ListNum\" FROM OPLN WHERE \"U_SEI_PS_ListNum\" = " + psListNum;
                    SQL.DoQuery(query);

                    if (SQL.count < 1)
                    {
                        return BadRequest("List Num doesn't exits");
                    }

                    Int16 sapListNum = SQL.dt.Rows[0].Field<Int16>("ListNum");
                    customer.ListNum = sapListNum;
                }
               
                
                jsonBody = JsonConvert.SerializeObject(customer, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                statusCode = SAP.CallServiceLayer(ref responseContent, SessionId, ServiceLayerEnum.InterlocutorComercial, Method.POST, jsonBody);
               

                if(statusCode == HttpStatusCode.Created)
                {
                    customer = JsonConvert.DeserializeObject<BusinessPartners> (responseContent);
                    if(psListNum != null)
                    {
                        customer.ListNum = psListNum;
                    }
                    return StatusCode((int)statusCode, customer);
                }
                else
                {
                    return StatusCode((int)statusCode, responseContent);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] BusinessPartners customer)
        {
            string responseContent = string.Empty;
            string jsonBody = string.Empty;
            HttpStatusCode? statusCode = null;

            ActionResult response = null;
            if (!CheckAndLogin(ref response))
            {
                return response;
            }

            

            try
            {
                customer.ListName = null;
                customer.GroupName = null;
                customer.CreateDateTime = null;
                customer.UpdateDateTime = null;

                if (customer.Addresses.Count > 0)
                {
                    customer.Addresses.ForEach(addr =>
                    {
                        addr.CardCode = id;
                    });
                }

                if (customer.ListNum != null)
                {
                    Int16 psListNum = (Int16)customer.ListNum;
                    
                    string query = "SELECT \"ListNum\" FROM OPLN WHERE \"U_SEI_PS_ListNum\" = " + psListNum;
                    SQL.DoQuery(query);

                    if (SQL.count < 1)
                    {
                        return BadRequest("List Num doesn't exits");
                    }

                    Int16 sapListNum = SQL.dt.Rows[0].Field<Int16>("ListNum");
                    customer.ListNum = sapListNum;

                }

                //Format Model
                jsonBody = JsonConvert.SerializeObject(customer, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                statusCode = SAP.CallServiceLayer(ref responseContent, SessionId, ServiceLayerEnum.InterlocutorComercial, Method.PATCH, jsonBody, id);

                if (statusCode == HttpStatusCode.NoContent)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode((int)statusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckAndLogin(ref ActionResult response)
        {
            if (token != Configuration["token"])
            {
                response =  Unauthorized("Invalid Token");
                return false;
            }
            else
            {
                if(!SAP.LoginSL(ref SessionId))
                {
                    response = Unauthorized("Login Failed.");
                    return false;
                };

            };

            return true;
        }

        private void AddDirection(DataRow row, BusinessPartners customer)
        {
            if(row.Field<string>("Address") is null)
            {
                return;
            }

            Direction direction = new Direction();
            direction.AddressCode = row.Field<string?>("AddressCode");
            direction.Number = row.Field<int?>("LineNum");
            direction.CardCode = row.Field<string?>("CardCode");
            direction.AddressType = row.Field<string?>("AdresType");
            direction.AddressName = row.Field<string?>("Address");
            direction.Street = row.Field<string?>("Street");
            direction.ZipCode = row.Field<string?>("ZipCode");
            direction.City = row.Field<string?>("City");
            direction.County = row.Field<string?>("County");
            direction.Country = row.Field<string?>("Country");
            direction.Phone = row.Field<string?>("Adress_phone");

            customer.Addresses.Add(direction);
        }

    }
}
