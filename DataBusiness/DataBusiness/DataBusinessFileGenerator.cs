using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataBusiness
{
    public class DataBusinessFileGenerator
    {

        public static void GenerateUsingLibraries(StreamWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");
            writer.WriteLine("using System.Threading.Tasks;");
            writer.WriteLine("using DataAccess;");
        }

        public static void GenerateDataBusinessFile(string tableName, string outputPath)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName;
            string DTOName = EntityName + "DTO";

            string filePath = Path.Combine(outputPath, $"{ClassName}.cs");

            using (StreamWriter writer = new StreamWriter(filePath))
            {

                GenerateUsingLibraries(writer);
                writer.WriteLine();
                writer.WriteLine("namespace DataBusiness");
                writer.WriteLine("{"); // start namespace

                DataBusinessClassGenerator.GenerateDataBusinessClass(writer, tableName);

                writer.WriteLine("}"); // End namespace
            }
        }

    }
}
