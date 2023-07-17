using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sanicompras.Classes;
using Sanicompras.Models;
using System.Data;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        [FromHeader(Name = "token")] public string token { get; set; }
        private ServiceLayer SAP = new ServiceLayer();
        private string SessionId = string.Empty;


        public OrdersController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);

        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<Documents>>> Get([FromQuery] Filter filter)
        {
            //string BaseURL = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }


            string query = Querys.getQuery(SQLFiles.Orders_GetAll);
            string offsset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offsset = $"ORDER BY t1.\"DocEntry\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }
            query = string.Format(query, offsset);

            PagedCollection<Documents> documents = new PagedCollection<Documents>();
            Documents currentOrder = null;
            int? currentDocEntry = null;

            try
            {

                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        if (row.Field<int>("DocEntry") != currentDocEntry)
                        {
                            if (currentOrder != null)
                            {
                                documents.Data.Add(currentOrder);
                            }

                            currentOrder = new Documents();
                            currentOrder.DocEntry = row.Field<int?>("DocEntry");
                            currentOrder.DocNum = row.Field<int?>("DocNum");
                            currentOrder.DocDate = row.Field<DateTime?>("DocDate");
                            currentOrder.DocDueDate = row.Field<DateTime?>("DocDueDate");
                            currentOrder.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentOrder.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");
                            currentOrder.DocTotal = row.Field<decimal?>("DocTotal");
                            currentOrder.DocStatus = row.Field<string?>("DocStatus");
                            currentOrder.Canceled = row.Field<string?>("CANCELED");
                            currentOrder.CardCode = row.Field<string?>("CardCode");
                            //...

                            currentDocEntry = row.Field<int?>("DocEntry");

                            AddLine(row, currentOrder);
                        }
                        else
                        {
                            AddLine(row, currentOrder);
                        }

                    }
                    documents.NextPage = filter.GetNextPage(Request);
                    documents.PreviousPage = filter.GetPreviousPage(Request);
                    documents.Data.Add(currentOrder);
                    return Ok(documents);
                }
                else
                {
                    return NotFound(documents);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documents>> Get(int id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            Documents document = new Documents();
            string query = Querys.getQuery(SQLFiles.Orders_GetByID);
            query = string.Format(query, id);

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    document.DocEntry = SQL.dt.Rows[0].Field<int?>("DocEntry");
                    document.DocNum = SQL.dt.Rows[0].Field<int?>("DocNum");
                    document.DocDate = SQL.dt.Rows[0].Field<DateTime?>("DocDate");
                    document.DocDueDate = SQL.dt.Rows[0].Field<DateTime?>("DocDueDate");
                    document.CreateDateTime = SQL.dt.Rows[0].Field<DateTime?>("CreateDateTime");
                    document.UpdateDateTime = SQL.dt.Rows[0].Field<DateTime?>("UpdateDateTime");
                    document.DocTotal = SQL.dt.Rows[0].Field<decimal?>("DocTotal");
                    document.DocStatus = SQL.dt.Rows[0].Field<string?>("DocStatus");
                    document.Canceled = SQL.dt.Rows[0].Field<string?>("CANCELED");
                    document.CardCode = SQL.dt.Rows[0].Field<string?>("CardCode");
                    //stock.HashCode = stock.GetHashCode().ToString();

                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        AddLine(row, document);
                    }

                    return Ok(document);
                }
                else
                {
                    return NotFound(document);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetByDate")]
        public async Task<ActionResult<PagedCollection<Documents>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            //if(StartDate == null && EndDate == null)
            //{
            //    return BadRequest("StartDate and EndDate can't not be empty at the same time");
            //}

            string query = Querys.getQuery(SQLFiles.Orders_GetByDate);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t.\"DocEntry\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<Documents> documents = new PagedCollection<Documents>();
            Documents currentOrder = null;
            int? currentDocEntry = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        if (row.Field<int>("DocEntry") != currentDocEntry)
                        {
                            if (currentOrder != null)
                            {
                                documents.Data.Add(currentOrder);
                            }

                            currentOrder = new Documents();
                            currentOrder.DocEntry = row.Field<int?>("DocEntry");
                            currentOrder.DocNum = row.Field<int?>("DocNum");
                            currentOrder.DocDate = row.Field<DateTime?>("DocDate");
                            currentOrder.DocDueDate = row.Field<DateTime?>("DocDueDate");
                            currentOrder.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentOrder.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");
                            currentOrder.DocTotal = row.Field<decimal?>("DocTotal");
                            currentOrder.DocStatus = row.Field<string?>("DocStatus");
                            currentOrder.Canceled = row.Field<string?>("CANCELED");
                            currentOrder.CardCode = row.Field<string?>("CardCode");
                            //...

                            currentDocEntry = row.Field<int?>("DocEntry");

                            AddLine(row, currentOrder);
                        }
                        else
                        {
                            AddLine(row, currentOrder);
                        }

                    }
                    //string startDateQuery = StartDate != null ? $"&StartDate={StartDate}" : "";
                    //string endDateQuery = StartDate != null ? $"&EndDate={EndDate}" : "";
                    documents.NextPage = filter.GetNextPage(Request);
                    documents.PreviousPage = filter.GetPreviousPage(Request);
                    documents.Data.Add(currentOrder);
                    Console.WriteLine(Request.Query);
                    foreach (string key in Request.Query.Keys)
                    {
                        Console.WriteLine(key + "__" + Request.Query[key]);

                    }
                    return Ok(documents);
                }
                else
                {
                    return NotFound(documents);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST api/<OrderController>
        [HttpPost]
        public ActionResult Post([FromBody] Orders order)
        {
            string responseContent = string.Empty;
            string jsonBody = string.Empty;
            HttpStatusCode? statusCode = null;
            Documents oDoc = null;

            ActionResult response = null;
            if (!CheckAndLogin(ref response))
            {
                return response;
            };

            try
            {
                if (order.Custumer.CardCode == "" || order.Custumer.CardCode == null)
                {

                    if (order.Custumer.Phone1 == null || order.Custumer.EmailAddress == null)
                    {
                        return BadRequest("Phone or Email can not be empty!");
                    }

                    order.Custumer.Series = int.Parse(Configuration["CustomerSeries"]);
                    order.Custumer.CardType = "C";
                    jsonBody = JsonConvert.SerializeObject(order.Custumer, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    statusCode = SAP.CallServiceLayer(ref responseContent, SessionId, ServiceLayerEnum.InterlocutorComercial, Method.POST, jsonBody);
                    if (statusCode == HttpStatusCode.Created)
                    {
                        order.Custumer.CardCode = (string)JObject.Parse(responseContent)["CardCode"];
                    }
                    else
                    {
                        return StatusCode((int)statusCode, responseContent);
                    }
                }

                oDoc = OrderToDoc(order);

                jsonBody = JsonConvert.SerializeObject(oDoc, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                statusCode = SAP.CallServiceLayer(ref responseContent, SessionId, ServiceLayerEnum.Pedidos, Method.POST, jsonBody);


                if (statusCode == HttpStatusCode.Created)
                {
                    oDoc = JsonConvert.DeserializeObject<Documents>(responseContent);
                    return StatusCode((int)statusCode, oDoc);
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

        private void AddLine(DataRow row, Documents order)
        {
            DocumentLine line = new DocumentLine();

            line.ItemCode = row.Field<string?>("ItemCode");
            line.ItemDescription = row.Field<string?>("Dscription");
            line.UnitPrice = row.Field<decimal?>("Price");
            line.Quantity = row.Field<decimal?>("Quantity");
            line.DiscountPercent = row.Field<decimal?>("DiscPrcnt");
            line.VatGroup = row.Field<string?>("VatGroup");
            line.LineTotal = row.Field<decimal?>("LineTotal");

            order.DocumentLines.Add(line);
        }

        //private bool existsCustumer(string cardCode)
        //{
        //    string query = "SELECT \"CardCode\" FROM OCRD WHERE \"CardCode\" = '" + cardCode + "'";
        //    try
        //    {
        //        SQL.DoQuery(query);
        //        if(SQL.count < 1){
        //            return false;
        //        }
        //        return true;
        //    }catch(Exception e)
        //    {
        //        throw e;
        //    }
        //}

        private Documents OrderToDoc(Orders order)
        {
            Documents doc = new Documents();
            doc.DocDate = order.DocDate;
            doc.DocDueDate = order.DocDueDate;
            doc.DocTotal = order.DocTotal;
            doc.CardCode = order.Custumer.CardCode;
            //doc.xxxx = order.SanicomprasID

            doc.DocumentLines = order.DocumentLines;
            return doc;
        }



        private bool CheckAndLogin(ref ActionResult response)
        {
            if (token != Configuration["token"])
            {
                response = Unauthorized("Invalid Token");
            }
            else
            {
                if (!SAP.LoginSL(ref SessionId))
                {
                    response = Unauthorized("Login Failed.");
                    return false;
                };

            };

            return true;
        }

    }
}
