using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Odbc;

namespace Sanicompras.Controllers
{
    public class SQLController : ControllerBase
    {
        public OdbcConnection conn;
        public OdbcDataAdapter da;
        public OdbcCommand cmd;
        public DataTable dt;
        public int count;
        public string exception;

        public SQLController(string connection)
        {
            conn = new OdbcConnection(connection);
        }


        public void DoQuery(string query)
        {
            count = 0;
            exception = "";

            try
            {
                cmd = new OdbcCommand(query, conn);

                dt = new DataTable();

                da = new OdbcDataAdapter(cmd);

                count = da.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

            }

        }

        public bool HasException()
        {
            if (string.IsNullOrEmpty(exception))
            {
                return false;
            }
            return true;
        }

    }
}
