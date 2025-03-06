using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.API
{
    public class APIFileGenerator
    {

        public static void GenerateUsingLibraries(StreamWriter writer)
        {
            writer.WriteLine("using Microsoft.AspNetCore.Authorization;");
            writer.WriteLine("using Microsoft.AspNetCore.Http;");
            writer.WriteLine("using System.Threading.Tasks;");
            writer.WriteLine("using Microsoft.AspNetCore.Mvc;");
            writer.WriteLine("using DataAccess;");
            writer.WriteLine("using DataBusiness;");
        }

        public static void GenerateAPIFile(string tableName, string outputPath)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName;
            string DTOName = EntityName + "DTO";


            string filePath = Path.Combine(outputPath, $"{EntityName}sController.cs");

            using (StreamWriter writer = new StreamWriter(filePath))
            {

                GenerateUsingLibraries(writer);
                writer.WriteLine();
                writer.WriteLine("namespace API.Controllers");
                writer.WriteLine("{"); // start namespace

                writer.WriteLine($"{Helper.Tabs(1)}//[Route(\"api/[controller]\")]");
                writer.WriteLine($"{Helper.Tabs(1)}[Route(\"api/{EntityName}s\")]");
                writer.WriteLine($"{Helper.Tabs(1)}[ApiController]");

                ControllerGenerator.GenerateController(writer, tableName);

                writer.WriteLine("}"); // End namespace
            }
        }

    }

}
