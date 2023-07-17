using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sanicompras.Models;
using System.Data;


namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        //private ServiceLayer SAP = new ServiceLayer();
        [FromHeader(Name = "token")] public string token { get; set; }

        public PricesController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);
        }

        [HttpGet]
        public async Task<ActionResult<PagedCollection<Prices>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }


            string query = Querys.getQuery(SQLFiles.Prices_GetAll);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t.\"ItemCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }

            query = string.Format(query, offset);

            PagedCollection<Prices> prices = new PagedCollection<Prices>();
            Prices currentItem = null;

            try
            {
                SQL.DoQuery(query);
                if (SQL.count > 0)
                {
                    string? currentItemCode = null;

                    foreach (DataRow row in SQL.dt.Rows)
                    {

                        if (row.Field<string?>("ItemCode") != currentItemCode)
                        {
                            if (currentItem != null)
                            {
                                //currentStock.HashCode = currentStock.GetHashCode().ToString();
                                prices.Data.Add(currentItem);
                            }

                            currentItem = new Prices();
                            currentItem.ItemCode = row.Field<string?>("ItemCode");
                            currentItem.ItemName = row.Field<string?>("ItemName");
                            currentItem.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentItem.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");


                            currentItemCode = row.Field<string?>("ItemCode");

                            AddItemToPriceList(row, currentItem);
                        }
                        else
                        {
                            AddItemToPriceList(row, currentItem);
                        }

                        //AddStockInWhareHouse(row, currentStock);

                    }

                    prices.NextPage = filter.GetNextPage(Request);
                    prices.PreviousPage = filter.GetPreviousPage(Request);
                    prices.Data.Add(currentItem);
                    return Ok(prices);
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
        public async Task<ActionResult<PagedCollection<Prices>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Prices_GetByDate);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t.\"ItemCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<Prices> prices = new PagedCollection<Prices>();
            Prices currentItem = null;

            try
            {
                SQL.DoQuery(query);
                if (SQL.count > 0)
                {
                    string? currentItemCode = null;

                    foreach (DataRow row in SQL.dt.Rows)
                    {

                        if (row.Field<string?>("ItemCode") != currentItemCode)
                        {
                            if (currentItem != null)
                            {
                                //currentStock.HashCode = currentStock.GetHashCode().ToString();
                                prices.Data.Add(currentItem);
                            }

                            currentItem = new Prices();
                            currentItem.ItemCode = row.Field<string?>("ItemCode");
                            currentItem.ItemName = row.Field<string?>("ItemName");
                            currentItem.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentItem.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");


                            currentItemCode = row.Field<string?>("ItemCode");

                            AddItemToPriceList(row, currentItem);
                        }
                        else
                        {
                            AddItemToPriceList(row, currentItem);
                        }

                        //AddStockInWhareHouse(row, currentStock);

                    }

                    prices.NextPage = filter.GetNextPage(Request);
                    prices.PreviousPage = filter.GetPreviousPage(Request);
                    prices.Data.Add(currentItem);
                    return Ok(prices);
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


       //GET: Price by ID
       [HttpGet("{id}")]
        public async Task<ActionResult<Prices>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Prices_GetByID);
            query = string.Format(query, id);

            Prices itemPrice = new Prices();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    itemPrice.ItemCode = SQL.dt.Rows[0].Field<string?>("ItemCode");
                    itemPrice.ItemName = SQL.dt.Rows[0].Field<string?>("ItemName");
                    itemPrice.CreateDateTime = SQL.dt.Rows[0].Field<DateTime?>("CreateDateTime");
                    itemPrice.UpdateDateTime = SQL.dt.Rows[0].Field<DateTime?>("UpdateDateTime");

                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        AddItemToPriceList(row, itemPrice);
                    }

                    //HashCode = stock.GetHashCode().ToString();

                    return Ok(itemPrice);
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


        private void AddItemToPriceList(DataRow row, Prices? price)
        {
            PriceList priceList = new PriceList();

            priceList.ListNum = row.Field<Int16?>("ListNum");
            priceList.ListName = row.Field<string?>("ListName");
            priceList.Price = row.Field<decimal?>("Price");
            priceList.Currency = row.Field<string?>("Currency");

            price.PriceLists.Add(priceList);
        }

    }
}
