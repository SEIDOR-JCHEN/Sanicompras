using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sanicompras.Models;
using System.Data;

namespace Sanicompras.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PricesB2BController : ControllerBase
    {
        //AppSettings
        private IConfiguration Configuration;
        private SQLController SQL;
        [FromHeader(Name = "token")] public string token { get; set; }

        public PricesB2BController(IConfiguration configuration)
        {
            Configuration = configuration;
            SQL = new SQLController(Configuration["ConnectionString"]);
        }

        //[HttpGet]
        //public async Task<ActionResult<PagedCollection<PricesB2B>>> Get([FromQuery] Filter filter)
        //{
        //    if (token != Configuration["token"])
        //    {
        //        return Unauthorized("Invalid Token");
        //    }

        //    string query = Querys.getQuery(SQLFiles.PricesB2B_GetAll);
        //    string offset = string.Empty;

        //    if (filter.Limit != 0)
        //    {
        //        int? page = filter.Page == 0 ? 1 : filter.Page;
        //        page = (page - 1) * filter.Limit;
        //        offset = $"ORDER BY t1.\"CardCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
        //    }

        //    query = string.Format(query, offset);

        //    PagedCollection<PricesB2B> priceB2Bs = new PagedCollection<PricesB2B>();
        //    PricesB2B currentPriceB2B = null;
        //    string currentCardCode = null;

        //    try
        //    {
        //        SQL.DoQuery(query);

        //        if (SQL.count > 0)
        //        {
        //            foreach (DataRow row in SQL.dt.Rows)
        //            {

        //                if (row.Field<string>("CardCode") != currentCardCode)
        //                {
        //                    if (currentPriceB2B != null)
        //                    {
        //                        //currentStock.HashCode = currentStock.GetHashCode().ToString();
        //                        priceB2Bs.Data.Add(currentPriceB2B);
        //                    }

        //                    currentPriceB2B = new PricesB2B();
        //                    currentPriceB2B.CardCode = row.Field<string>("CardCode");
        //                    currentPriceB2B.CardName = row.Field<string>("CardName");
        //                    currentPriceB2B.CreateDateTime = row.Field<DateTime>("CreateDateTime");
        //                    currentPriceB2B.UpdateDateTime = row.Field<DateTime>("UpdateDateTime");

        //                    currentCardCode = row.Field<string>("CardCode");

        //                    AddSpecialPrice(row, currentPriceB2B);
        //                }
        //                else
        //                {
        //                    AddSpecialPrice(row, currentPriceB2B);
        //                }
        //            }

        //            priceB2Bs.NextPage = filter.GetNextPage(Request);
        //            priceB2Bs.PreviousPage = filter.GetPreviousPage(Request);
        //            priceB2Bs.Data.Add(currentPriceB2B);
        //            return Ok(priceB2Bs);
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        [HttpGet("GetByDate")]
        public async Task<ActionResult<PagedCollection<PricesB2B>>> Get([FromQuery] Filter filter, [FromQuery] string? StartDate, [FromQuery] string? EndDate)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.PricesB2B_GetByDate);
            string offset = string.Empty;

            if (filter.Limit != 0)
            {
                int? page = filter.Page == 0 ? 1 : filter.Page;
                page = (page - 1) * filter.Limit;
                offset = $"ORDER BY t1.\"CardCode\" OFFSET {page} ROW FETCH NEXT {filter.Limit} ROWS ONLY ";
            }
            EndDate = EndDate == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : EndDate;
            query = string.Format(query, StartDate, EndDate, offset);

            PagedCollection<PricesB2B> priceB2Bs = new PagedCollection<PricesB2B>();
            PricesB2B currentPriceB2B = null;
            string currentCardCode = null;

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    foreach (DataRow row in SQL.dt.Rows)
                    {

                        if (row.Field<string>("CardCode") != currentCardCode)
                        {
                            if (currentPriceB2B != null)
                            {
                                //currentStock.HashCode = currentStock.GetHashCode().ToString();
                                priceB2Bs.Data.Add(currentPriceB2B);
                            }

                            currentPriceB2B = new PricesB2B();
                            currentPriceB2B.CardCode = row.Field<string?>("CardCode");
                            currentPriceB2B.CardName = row.Field<string?>("CardName");
                            currentPriceB2B.CreateDateTime = row.Field<DateTime?>("CreateDateTime");
                            currentPriceB2B.UpdateDateTime = row.Field<DateTime?>("UpdateDateTime");

                            currentCardCode = row.Field<string?>("CardCode");

                            AddSpecialPrice(row, currentPriceB2B);
                        }
                        else
                        {
                            AddSpecialPrice(row, currentPriceB2B);
                        }
                    }

                    priceB2Bs.NextPage = filter.GetNextPage(Request);
                    priceB2Bs.PreviousPage = filter.GetPreviousPage(Request);
                    priceB2Bs.Data.Add(currentPriceB2B);
                    return Ok(priceB2Bs);
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
        public async Task<ActionResult<PricesB2B>> Get(string id)
        {
            if (token != Configuration["token"])
            {
                return Unauthorized("Invalid Token");
            }

            string query = Querys.getQuery(SQLFiles.PricesB2B_GetByID);
            query = string.Format(query, id);

            PricesB2B priceB2B = new PricesB2B();

            try
            {
                SQL.DoQuery(query);

                if (SQL.count > 0)
                {
                    priceB2B.CardCode = SQL.dt.Rows[0].Field<string?>("CardCode");
                    priceB2B.CardName = SQL.dt.Rows[0].Field<string?>("CardName");
                    priceB2B.CreateDateTime = SQL.dt.Rows[0].Field<DateTime?>("CreateDateTime");
                    priceB2B.UpdateDateTime = SQL.dt.Rows[0].Field<DateTime?>("UpdateDateTime");


                    foreach (DataRow row in SQL.dt.Rows)
                    {
                        AddSpecialPrice(row, priceB2B);
                    }

                    //HashCode = stock.GetHashCode().ToString();

                    return Ok(priceB2B);
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

        private void AddSpecialPrice(DataRow row, PricesB2B priceB2B)
        {
            SpecialPrice specialPrice = new SpecialPrice();
            specialPrice.ItemCode = row.Field<string?>("ItemCode");
            specialPrice.ItemName = row.Field<string?>("ItemName");
            specialPrice.Price = row.Field<decimal?>("Price");
            specialPrice.Discount = row.Field<decimal?>("Discount");
            specialPrice.ListNum = row.Field<Int16?>("ListNum");

            priceB2B.SpecialPrices.Add(specialPrice);
        }
    }
}
