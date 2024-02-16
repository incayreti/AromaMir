using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AromatnyMir
{
    internal class db
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=HOME-PC\MSSQLSERVER01;Initial Catalog=AromaMir;Integrated Security=True;TrustServerCertificate=True");

        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed) { sqlConnection.Open(); }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open) { sqlConnection.Close(); }
        }

        public SqlConnection getConnection() { return sqlConnection; }
    }
}
