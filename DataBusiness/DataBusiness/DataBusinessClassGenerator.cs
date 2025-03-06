using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataBusiness
{
    public class DataBusinessClassGenerator
    {

        private static string GenerateDataBusinessClassProperties(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string DataBusinessClassProperties = "";
            foreach (var column in columns)
            {
                if (columns.Last() == column)
                {
                    DataBusinessClassProperties += $"{Helper.Tabs(2)}public {column.DataType} {column.ColumnName} {{ get; set; }}";
                }
                else
                {
                    DataBusinessClassProperties += $"{Helper.Tabs(2)}public {column.DataType} {column.ColumnName} {{ get; set; }}\n";
                }
            }
            return DataBusinessClassProperties;
        }

        private static string GenerateDTOConstructorArguments(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string DTOConstructorArgs = "";
            foreach (var column in columns)
            {
                DTOConstructorArgs += $"this.{column.ColumnName}, ";
            }
            DTOConstructorArgs = DTOConstructorArgs.TrimEnd(',', ' ');
            return DTOConstructorArgs;
        }

        private static string GenerateDTOConstructorAssignments(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            string DataBusinessClassConstructorAssignments = "";
            foreach (var column in columns)
            {
                if (columns.Last() == column)
                {
                    DataBusinessClassConstructorAssignments += $"{Helper.Tabs(3)}this.{column.ColumnName} = {DTOName}.{column.ColumnName};";
                }
                else
                {
                    DataBusinessClassConstructorAssignments += $"{Helper.Tabs(3)}this.{column.ColumnName} = {DTOName}.{column.ColumnName};\n";
                }
            }
            return DataBusinessClassConstructorAssignments;
        }

        static void GetPropertySection(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            writer.WriteLine($"{Helper.Tabs(2)}public enum enMode {{AddNew = 0,Update = 1}};");
            writer.WriteLine($"{Helper.Tabs(2)}public enMode Mode = enMode.AddNew;");
            writer.WriteLine(GenerateDataBusinessClassProperties(tableName));
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(2)}public {DTOName} {DTOName}");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start DTO Prop
            writer.WriteLine($"{Helper.Tabs(3)}get");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start DTO Prop get
            writer.WriteLine($"{Helper.Tabs(4)}return new {DTOName}({GenerateDTOConstructorArguments(tableName)});");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End DTO Prop get
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End DTO Prop 
        }

        static void GenerateDataBusinessConstructorSection(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            writer.WriteLine($"{Helper.Tabs(2)}public {ClassName}({DTOName} {DTOName}, enMode Mode = enMode.AddNew)");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Data Business Class Constructor
            writer.WriteLine(GenerateDTOConstructorAssignments(tableName));
            writer.WriteLine($"{Helper.Tabs(3)}this.Mode = Mode;"); //
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Data Business Class Constructor
        }


        static void GenerateCreateMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}public bool _AddNew{EntityName}()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add
            //writer.WriteLine($"{Helper.Tabs(3)}this.Id = {ClassName}Data.Add{EntityName}({DTOName});");
            writer.WriteLine($"{Helper.Tabs(3)}this.{ColumnToCheck.ColumnName} = {ClassName}Data.Add{EntityName}({DTOName});");
            //writer.WriteLine($"{Helper.Tabs(3)}return (this.Id != -1);");
            writer.WriteLine($"{Helper.Tabs(3)}return (this.{ColumnToCheck.ColumnName} != -1);");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add
        }

        static void GenerateReadAllMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            writer.WriteLine($"{Helper.Tabs(2)}public static List<{DTOName}> GetAll{EntityName}s()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get All
            writer.WriteLine($"{Helper.Tabs(3)}return {ClassName}Data.GetAll{EntityName}s();");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get All
        }

        static void GenerateReadMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            //writer.WriteLine($"{Helper.Tabs(2)}public static {ClassName} Find(int Id)");
            writer.WriteLine($"{Helper.Tabs(2)}public static {ClassName} Find({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get By Id
            //writer.WriteLine($"{Helper.Tabs(3)}{DTOName} {DTOName} = {ClassName}Data.Get{EntityName}ById(Id);");
            writer.WriteLine($"{Helper.Tabs(3)}{DTOName} {DTOName} = {ClassName}Data.Get{EntityName}By{Helper.Capitalize(ColumnToCheck.ColumnName)}({ColumnToCheck.ColumnName});");
            writer.WriteLine($"{Helper.Tabs(3)}if ({DTOName} != null)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return new {ClassName}({DTOName}, enMode.Update);");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine($"{Helper.Tabs(3)}else");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start else
            writer.WriteLine($"{Helper.Tabs(4)}return null;");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End else
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get By Id
        }

        static void GenerateUpdateMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}public bool _Update{EntityName}()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Update
            writer.WriteLine($"{Helper.Tabs(3)}return {ClassName}Data.Update{EntityName}({DTOName});");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Update
        }

        static void GenerateDeleteMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}public static bool Delete{EntityName}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add
            writer.WriteLine($"{Helper.Tabs(3)}return {ClassName}Data.Delete{EntityName}({ColumnToCheck.ColumnName});");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add            
        }

        static void GenerateSave(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            writer.WriteLine($"{Helper.Tabs(2)}public bool Save()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add
            writer.WriteLine($"{Helper.Tabs(3)}switch (Mode)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start switch
            writer.WriteLine($"{Helper.Tabs(4)}case enMode.AddNew:");
            writer.WriteLine($"{Helper.Tabs(5)}if (_AddNew{EntityName}())");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(6)}Mode = enMode.Update;");
            writer.WriteLine($"{Helper.Tabs(6)}return true;");
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End if
            writer.WriteLine($"{Helper.Tabs(5)}else");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start else
            writer.WriteLine($"{Helper.Tabs(6)}return false;");
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End else
            writer.WriteLine($"{Helper.Tabs(4)}case enMode.Update:");
            writer.WriteLine($"{Helper.Tabs(5)}return _Update{EntityName}();");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End switch
            writer.WriteLine($"{Helper.Tabs(3)}return false;");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add
        }

        public static void GenerateDataBusinessClass(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName;
            string DTOName = EntityName + "DTO";
            writer.WriteLine($"{Helper.Tabs(1)}public class {ClassName}");
            writer.WriteLine($"{Helper.Tabs(1)}{{"); // Start Data Business Class
            writer.WriteLine();

            GetPropertySection(writer, tableName);

            writer.WriteLine();
            GenerateDataBusinessConstructorSection(writer, tableName);
            // Add
            writer.WriteLine();
            GenerateCreateMethod(writer, tableName);
            // Update
            writer.WriteLine();
            GenerateUpdateMethod(writer, tableName);
            // Get All
            writer.WriteLine();
            GenerateReadAllMethod(writer, tableName);
            // Get By Id
            writer.WriteLine();
            GenerateReadMethod(writer, tableName);
            // Save
            writer.WriteLine();
            GenerateSave(writer, tableName);
            // DeleteBook
            writer.WriteLine();
            GenerateDeleteMethod(writer, tableName);

            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(1)}}}"); // End Data Business Class
        }

    }
}
