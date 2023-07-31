using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogLibrary.DBConnect
{
    public class DBConnector
    {
        public SqlConnection SqlCnn;

        public DBConnector(string cnnString)
        {
            SqlCnn = new(cnnString);
           
        }

        public bool ConnectToDB()
        {

            bool IsDbOk = false;

            try
            {
                if (SqlCnn.State == ConnectionState.Closed)
                {
                    SqlCnn.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella Connessione al DB: {ex.Message}");
                return IsDbOk = false;
            }

            return IsDbOk = true;

        }
    }
}
