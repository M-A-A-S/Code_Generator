using DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DataAccessGenerator
    {

        private static string _OutputPath = @"D:\GeneratedClasses\DataAccess";

        public static void Generate(string DatabaseName, string OutputPath)
        {
            _OutputPath = OutputPath;
            Global.ConnectionString = $"Server=.;Database={DatabaseName};User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

            // Ensure the output directory exists
            Directory.CreateDirectory(_OutputPath);

            DataAccessSettingsFileGenerator.GenerateDataAccessSettingsFile(DatabaseName, _OutputPath);

            // Connect to the database
            using (SqlConnection connection = new SqlConnection(Global.ConnectionString))
            {
                connection.Open();

                // Get all table names
                List<string> tableNames = DatabaseHelper.GetTableNamesOfDatabase(Global.ConnectionString);

                // Generate classes and CRUD operations for each table
                foreach (var tableName in tableNames)
                {
                    // Get All columns of a table
                    List<ColumnSchema> columns = DatabaseHelper.GetTableSchemaForCSharp(Global.ConnectionString, tableName);
                    // Create The Controller Class
                    DataAccessFileGenerator.GenerateDataAccessFile(tableName, _OutputPath);
                }
            }

        }

    }
}
