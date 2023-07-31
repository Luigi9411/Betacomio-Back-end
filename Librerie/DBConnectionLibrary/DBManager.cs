using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Data.SqlTypes;

namespace DBConnectionLibrary
{
    public class DBManager
    {
        public SqlConnection connection = new();
        public bool isDBOk;

        public void createConnection(string connectionStr)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.ConnectionString = connectionStr;
                    connection.Open();
                    isDBOk = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nella connessione al database: " + ex.Message);
                isDBOk = false;
            }

        }

    }
}