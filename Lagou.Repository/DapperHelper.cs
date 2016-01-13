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

        
        private  string connectionString = "Data Source=192.168.0.19;Initial Catalog=HKLG;User ID=sa;Pwd=123456";
        //private readonly string connectionString = "Data Source=(local);Initial Catalog=HKLG;User ID=sa;Pwd=123456";
        //private readonly string connectionString = "Data Source=(local);Initial Catalog=HKLG;Integrated Security=True";


        public  DbConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

    }
}
