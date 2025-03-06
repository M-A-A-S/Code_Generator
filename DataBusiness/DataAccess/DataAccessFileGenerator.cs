using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DataAccessFileGenerator
    {

        public static void GenerateDataAccessLibrariesSection(StreamWriter writer)
        {
            writer.WriteLine("using Microsoft.Data.SqlClient;");
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Data;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Threading.Tasks;");
        }

        public static void GenerateDataAccessFile(string tableName, string outputPath)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName + "Data";
            string DTOName = EntityName + "DTO";

            string filePath = Path.Combine(outputPath, $"{ClassName}.cs");

            using (StreamWriter writer = new StreamWriter(filePath))
            {

                GenerateDataAccessLibrariesSection(writer);
                writer.WriteLine();
                writer.WriteLine("namespace DataAccess");
                writer.WriteLine("{"); // start namespace 
                Console.WriteLine();

                // start DTO
                DTOGenerator.GenerateDTOClass(writer, tableName);
                // End DTO

                writer.WriteLine();
                // start Data access class
                DataAccessClassGenerator.GenerateDataAccessClass(writer, tableName);
                // End Data access class

                Console.WriteLine();
                writer.WriteLine("}"); // End namespace
            }
        }
    }
}
