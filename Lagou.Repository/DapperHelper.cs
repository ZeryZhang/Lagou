using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Lagou.Repository
{
    public class DapperHelper
    {

        private static DbConnection connection;
        private  string connectionString = "Data Source=192.168.0.19;Initial Catalog=HKLG;User ID=sa;Pwd=123456";
        //private readonly string connectionString = "Data Source=Zery-Zhang;Initial Catalog=HKLG;User ID=sa;Pwd=123456";


        public  DbConnection GetConnection()
        {
            return connection = new SqlConnection(connectionString);
        }

    }
}
