
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace ErrorLogLibrary.BusinessLogic
{
    public class ErrorManager
    {
        SqlConnection sqlCnn;
        string LogPath;

        public ErrorManager(string cnn, string path)
        {

            sqlCnn = new(cnn);
            LogPath = path;
        }

        public bool ConnectToDB()
        {

            bool IsDbOk = false;

            try
            {
                if (sqlCnn.State == ConnectionState.Closed)
                {
                    sqlCnn.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella Connessione al DB: {ex.Message}");
                return IsDbOk = false;
            }

            return IsDbOk = true;

        }

        public void SaveException(string tableName,Exception ex, string sourceClass, string sourceMethod, DateTime errorDate, string extra)
        {

            ConnectToDB();

            StringBuilder sqlCmdText = new StringBuilder();
            sqlCmdText.Append($"INSERT INTO {tableName} ");
            sqlCmdText.Append("([ExceptionCode],[ExceptionDescription],[SourceClass], [SourceMethod],[ErrorDate],[ExtraInfos])");
            sqlCmdText.Append("VALUES\r\n");
            sqlCmdText.Append($"('{ex.HResult}', '{ex.Message.ToString()}', '{sourceClass}','{sourceMethod}','{DateTime.Now}', '{extra}')");
            //sqlCmdText.Append("('404', 'Prova ins da db', 'sql', 'sqlQuery', null, 'no info')");
            // string sqlCmdText = "Insert Into [dbo].[Errors]\r\n([ExceptionCode], [ExceptionDescription], [SourceClass], [SourceMethod], [ErrorDate],[ExtraInfos])\r\nValues ('404', 'Prova ins da db', 'sql', 'sqlQuery', null, 'no info')";

            try
            {
                SqlCommand cmd = sqlCnn.CreateCommand();
                cmd.CommandText = sqlCmdText.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }
            catch (Exception exception)
            {
               
                StringBuilder sb = new StringBuilder();
                sb.Append($"Exception Code: {exception.HResult};Date: {DateTime.Now}; Message: {exception.Message}; " +
                    $"Source Class: ErrorManager;Source Method: SaveException; Description: Something went wrong when writing exception log\n");
                
                File.AppendAllText(LogPath, sb.ToString());
                //Console.WriteLine("Ho fallito a scrivere nel db, errore: " + ex.Message);
            }
            finally
            {
                sqlCnn.Close();
            }

        }
    }
}
