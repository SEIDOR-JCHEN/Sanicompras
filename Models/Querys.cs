using System.Reflection;

namespace Sanicompras.Models
{
    public class Querys
    {
        public static string getQuery(string filename)
        {
            const string baseDir = "Sanicompras.SQL.";

            string query = string.Empty;
            Assembly assenbly = Assembly.GetExecutingAssembly();
            using (Stream stream = assenbly.GetManifestResourceStream(baseDir + filename))
            using (StreamReader reader = new StreamReader(stream))
            {
                query = reader.ReadToEnd();
            }
            return query;
        }
    }

    public struct SQLFiles
    {
        public const string Orders_GetAll = "Orders_GetAll.sql";
        public const string Orders_GetByDate = "Orders_GetByDate.sql";
        public const string Orders_GetByID = "Orders_GetByID.sql";

        public const string Customers_GetAll = "Customers_GetAll.sql";
        public const string Customers_GetByDate = "Customers_GetByDate.sql";
        public const string Customers_GetByID = "Customers_GetByID.sql";

        public const string CustomersGroup_GetAll = "CustomersGroup_GetAll.sql";
        public const string CustomersGroup_GetByID = "CustomersGroup_GetByID.sql";

        public const string Products_GetAll = "Products_GetAll.sql";
        public const string Products_GetByDate = "Products_GetByDate.sql";
        public const string Products_GetByID = "Products_GetByID.sql";
        public const string GetPriceLists = "GetPriceLists.sql";
        public const string GetPriceB2B = "GetPriceB2B.sql";
        public const string GetAlternatives = "GetAlternatives.sql";
        public const string GetProductParams = "GetProductParams.sql";
        public const string GetProductProps = "GetProductProps.sql";

        public const string Prices_GetAll = "Prices_GetAll.sql";
        public const string Prices_GetByDate = "Prices_GetByDate.sql";
        public const string Prices_GetByID = "Prices_GetByID.sql";

        public const string PricesB2B_GetAll = "PricesB2B_GetAll.sql";
        public const string PricesB2B_GetByDate = "PricesB2B_GetByDate.sql";
        public const string PricesB2B_GetByID = "PricesB2B_GetByID.sql";

        public const string Stocks_GetAll = "Stocks_GetAll.sql";
        public const string Stocks_GetByDate = "Stocks_GetByDate.sql";
        public const string Stocks_GetByID = "Stocks_GetByID.sql";

        public const string ProductFamily_GetAll = "ProductFamily_GetAll.sql";
        public const string ProductFamily_GetByDate = "ProductFamily_GetByDate.sql";
        public const string ProductFamily_GetByID = "ProductFamily_GetByID.sql";

        public const string ProductSubfamily_GetAll = "ProductSubfamily_GetAll.sql";
        public const string ProductSubfamily_GetByID = "ProductSubfamily_GetByID.sql";
    }
}
