using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DataAccessSettingsFileGenerator
    {
        public static void GenerateDataAccessSettingsFileLibrariesSection(StreamWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Threading.Tasks;");
        }

        public static void GenerateDataAccessSettingsFile(string DatabaseName, string outputPath)
        {

            string filePath = Path.Combine(outputPath, $"clsDataAccessSettings.cs");

            using (StreamWriter writer = new StreamWriter(filePath))
            {

                GenerateDataAccessSettingsFileLibrariesSection(writer);
                writer.WriteLine();
                writer.WriteLine("namespace DataAccess");
                writer.WriteLine("{"); // start namespace 

                // start DataAccess Settings
                DataAccessSettingsClassGenerator.GenerateDataAccessSettingsClass(writer, DatabaseName);
                // End DataAccess Settings

                writer.WriteLine("}"); // End namespace
            }
        }
    }
}
