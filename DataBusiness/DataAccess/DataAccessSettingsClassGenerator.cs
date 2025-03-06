using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DataAccessSettingsClassGenerator
    {
        public static void GenerateDataAccessSettingsClass(StreamWriter writer, string DatabaseName)
        {

            writer.WriteLine($"\tpublic class clsDataAccessSettings");
            writer.WriteLine("\t{"); // start DataAccess Settings
            writer.WriteLine();
            //writer.WriteLine($"\t\tpublic static string ConnectionString = \"Server=.;Database=DB1;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;\";");
            writer.WriteLine($"\t\tpublic static string ConnectionString = \"Server=.;Database={DatabaseName};User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;\";");
            Console.WriteLine();
            writer.WriteLine("\t}"); // End DataAccess Settings
        }

    }
}
