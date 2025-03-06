using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBusiness.API;
using DataBusiness.DataAccess;
using DataBusiness.DataBusiness;
using DataBusiness.StoredProcedures;

namespace DataBusiness
{
    public class CodeGenerator
    {

        public static void Generate(string DatabaseName, string OutputPath)
        {
            string DataAccessOutputPath = @$"{OutputPath}\DataAccess";
            string DataBusinessOutputPath = @$"{OutputPath}\DataBusiness";
            string APIOutputPath = @$"{OutputPath}\API";
            string ConnectionString = $"Server=.;Database={DatabaseName};User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";
            Global.ConnectionString = $"Server=.;Database={DatabaseName};User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

            if (DatabaseHelper.IsDatabaseExists(ConnectionString, DatabaseName))
            {
                StoredProcedureGenerator.Generate(DatabaseName);
                DataAccessGenerator.Generate(DatabaseName, DataAccessOutputPath);
                DataBusinessGenerator.Generate(DatabaseName, DataBusinessOutputPath);
                APIGenerator.Generate(DatabaseName, APIOutputPath);

                Console.WriteLine("Code generation complete!");
            }
            else
            {
                Console.WriteLine($"There is no DataBase With Name '{DatabaseName}'");
            }
        }
    }
}
