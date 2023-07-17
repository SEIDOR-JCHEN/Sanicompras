using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sanicompras.Models;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        [FromHeader(Name = "token")]
        public string token { get; set; }

        public StocksController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);
        }

        // GET: All Stocks
        [HttpGet]
        public async Task<ActionResult<PagedCollection<Stocks>>> Get([FromQuery] Filter filter)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Stocks_GetAll);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t1.\"ItemCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            query = string.Format(query, offset);

            PagedCollection<Stocks> stocks = new PagedCollection<Stocks>();
            Stocks currentStock = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    string currentItem = "";
                   
                    foreach (DataRow row in SQL.dt.Rows)
                    {

                        if (row.Field<string?>("ItemCode") != currentItem)
                        {
                            if(currentStock != null)
                            {
                                //currentStock.HashCode = currentStock.GetHashCode().ToString();
                                stocks.Data.Add(currentStock);
                            }

                            currentStock = new Stocks();
                            currentStock.ItemCode = row.Field<string?>("ItemCode");
                            currentStock.ItemName = row.Field<string?>("ItemName");
                            currentStock.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentStock.UpdateDatetime = row.Field<DateTime?>("UpdateDatetime");

                            currentItem = row.Field<string?>("ItemCode");

                            AddStockInWhareHouse(row, currentStock);
                        }
                        else
                        {
                            AddStockInWhareHouse(row, currentStock);
                        }

                        //AddStockInWhareHouse(row, currentStock);
                 
                    }

                    stocks.NextPage = filter.GetNextPage(Request);
                    stocks.PreviousPage = filter.GetPreviousPage(Request);
                    stocks.Data.Add(currentStock);
                    return Ok(stocks);
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
        public async Task<ActionResult<PagedCollection<Stocks>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.Stocks_GetByDate);
            string offset = string.Empty;
            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t.\"ItemCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<Stocks> stocks = new PagedCollection<Stocks>();
            Stocks currentStock = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    string currentItem = "";
                   
                    foreach (DataRow row in SQL.dt.Rows)
                    {

                        if (row.Field<string?>("ItemCode") != currentItem)
                        {
                            if(currentStock != null)
                            {
                                //currentStock.HashCode = currentStock.GetHashCode().ToString();
                                stocks.Data.Add(currentStock);
                            }

                            currentStock = new Stocks();
                            currentStock.ItemCode = row.Field<string?>("ItemCode");
                            currentStock.ItemName = row.Field<string?>("ItemName");
                            currentStock.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentStock.UpdateDatetime = row.Field<DateTime?>("UpdateDatetime");

                            currentItem = row.Field<string?>("ItemCode");

                            AddStockInWhareHouse(row, currentStock);
                        }
                        else
                        {
                            AddStockInWhareHouse(row, currentStock);
                        }

                        //AddStockInWhareHouse(row, currentStock);
                 
                    }

                    stocks.NextPage = filter.GetNextPage(Request);
                    stocks.PreviousPage = filter.GetPreviousPage(Request);
                    stocks.Data.Add(currentStock);
                    return Ok(stocks);
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


        // GET: Stock by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Stocks>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }
            string query = Querys.getQuery(SQLFiles.Stocks_GetByID);
            query = string.Format(query, id);
            
            Stocks stock = new Stocks();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    stock.ItemCode = SQL.dt.Rows[0].Field<string?>("ItemCode");
                    stock.ItemName = SQL.dt.Rows[0].Field<string?>("ItemName");
                    stock.CreateDateTime = SQL.dt.Rows[0].Field<DateTime?>("CreateDateTime");
                    stock.UpdateDatetime = SQL.dt.Rows[0].Field<DateTime?>("UpdateDatetime");
                    //stock.HashCode = stock.GetHashCode().ToString();

                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        AddStockInWhareHouse(row, stock);
                    }

                    return Ok(stock);
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

        private void AddStockInWhareHouse(DataRow row, Stocks stock)
        {
            StockInWhareHouse stockInWhareHouse = new StockInWhareHouse();

            stockInWhareHouse.WhsCode = row.Field<string?>("WhsCode");
            stockInWhareHouse.WhsName = row.Field<string?>("WhsName");
            stockInWhareHouse.Quantity = row.Field<decimal?>("Quantity");

            stock.StockInWhareHouses.Add(stockInWhareHouse);
        }

    }

}
