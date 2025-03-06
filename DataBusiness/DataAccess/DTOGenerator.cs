using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DTOGenerator
    {

        private static string GenerateDTOProperties(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string DTOProperties = "";
            foreach (var column in columns)
            {
                if (columns.Last() == column)
                {
                    DTOProperties += $"{Helper.Tabs(2)}public {column.DataType} {column.ColumnName} {{ get; set; }}";
                }
                else
                {
                    DTOProperties += $"{Helper.Tabs(2)}public {column.DataType} {column.ColumnName} {{ get; set; }}\n";
                }
            }
            return DTOProperties;
        }

        private static string GenerateDTOConstructorParams(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string DTOConstructorParams = "";
            foreach (var column in columns)
            {
                //DTOConstructorParams += $"{column.DataType} {column.ColumnName.ToLower()}, ";
                DTOConstructorParams += $"{column.DataType} {column.ColumnName}, ";
            }
            DTOConstructorParams = DTOConstructorParams.TrimEnd(',', ' ');
            return DTOConstructorParams;
        }

        private static string GenerateDTOConstructorAssignments(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string DTOConstructorAssignments = "";
            foreach (var column in columns)
            {
                if (columns.Last() == column)
                {
                    DTOConstructorAssignments += $"\t\t\tthis.{column.ColumnName} = {column.ColumnName};";
                }
                else
                {
                    DTOConstructorAssignments += $"\t\t\tthis.{column.ColumnName} = {column.ColumnName};\n";
                }
            }
            return DTOConstructorAssignments;
        }

        public static void GenerateDTOClass(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";

            writer.WriteLine($"\tpublic class {DTOName}");
            writer.WriteLine("\t{"); // start DTO
            Console.WriteLine();
            writer.WriteLine(GenerateDTOProperties(tableName));
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic {DTOName}({GenerateDTOConstructorParams(tableName)})");
            writer.WriteLine("\t\t{"); // start DTO Constructor
            writer.WriteLine(GenerateDTOConstructorAssignments(tableName));
            writer.WriteLine("\t\t}"); // End DTO Constructor
            Console.WriteLine();
            writer.WriteLine("\t}"); // End DTO
        }

    }
}
