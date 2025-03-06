using DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.StoredProcedures
{
    public class StoredProcedureGenerator
    {

        private static string InsertConstructorParams(List<ColumnSchema> columns, ColumnSchema ColumnToCheck)
        {
            string StoredProcedureConstructorParams = "";
            foreach (var column in columns)
            {
                if (column.ColumnName != ColumnToCheck.ColumnName)
                {
                    StoredProcedureConstructorParams += $"{column.ColumnName}, ";
                }
            }
            StoredProcedureConstructorParams = StoredProcedureConstructorParams.TrimEnd(',', ' ');
            return StoredProcedureConstructorParams;
        }

        private static string InsertValuesConstructorParams(List<ColumnSchema> columns, ColumnSchema ColumnToCheck)
        {
            string StoredProcedureConstructorParams = "";
            foreach (var column in columns)
            {
                if (column.ColumnName != ColumnToCheck.ColumnName)
                {
                    StoredProcedureConstructorParams += $"@{column.ColumnName}, ";
                }
            }
            StoredProcedureConstructorParams = StoredProcedureConstructorParams.TrimEnd(',', ' ');
            return StoredProcedureConstructorParams;
        }

        private static string GenerateStoredProcedureAssignments(string tableName, List<ColumnSchema> columns)
        {
            ColumnSchema ColumnToCheck;
            bool hasPrimaryKey = DatabaseHelper.HasPrimaryKey(tableName, Global.ConnectionString);
            if (hasPrimaryKey)
            {
                ColumnToCheck = DatabaseHelper.GetPrimaryKeyForTable(Global.ConnectionString, tableName);
            }
            ColumnToCheck = columns[0];

            string StoredProcedureAssignments = "";
            foreach (var column in columns)
            {
                if (column != ColumnToCheck)
                {
                    if (columns.Last() == column)
                    {
                        StoredProcedureAssignments += $"{Helper.Tabs(2)}{column.ColumnName} = @{column.ColumnName}";
                    }
                    else
                    {
                        StoredProcedureAssignments += $"{Helper.Tabs(2)}{column.ColumnName} = @{column.ColumnName},\n";
                    }
                }
            }
            return StoredProcedureAssignments;
        }

        public static void GenerateAndExecuteCRUDProcedures(string tableName, List<ColumnSchema> columns)
        {
            List<string> procedures = new List<string>
            {
                GenerateCreateProcedure(tableName, columns),
                GenerateReadAllProcedure(tableName),
                GenerateReadProcedure(tableName, columns),
                GenerateUpdateProcedure(tableName, columns),
                GenerateDeleteProcedure(tableName, columns)
            };

            foreach (var item in DatabaseHelper.GetPrimaryKeysForTable(Global.ConnectionString, tableName))
            {
                Console.WriteLine(item);
            }

            foreach (var procedure in procedures)
            {
                if (DatabaseHelper.ExecuteSqlCommand(procedure, Global.ConnectionString))
                {
                    Console.WriteLine("Procedure executed successfully.");
                }
                else
                {
                    Console.WriteLine($"Error executing procedure");
                }
            }
        }

        private static string GenerateCreateProcedure(string tableName, List<ColumnSchema> columns)
        {
            string EntityName = tableName.TrimEnd('s');

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE SP_AddNew{EntityName}");
            foreach (var column in columns)
            {
                if (column.ColumnName != ColumnToCheck.ColumnName)
                {
                    sb.AppendLine($"{Helper.Tabs(1)}@{column.ColumnName} {column.DataType},");
                }
            }

            //sb.AppendLine($"{Helper.Tabs(1)}@New{EntityName}Id int output");
            sb.AppendLine($"{Helper.Tabs(1)}@New{EntityName}{Helper.Capitalize(ColumnToCheck.ColumnName)} {ColumnToCheck.DataType} output");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"{Helper.Tabs(1)}INSERT INTO {tableName} ({InsertConstructorParams(columns, ColumnToCheck)})");
            sb.AppendLine($"{Helper.Tabs(1)}VALUES ({InsertValuesConstructorParams(columns, ColumnToCheck)});");
            //sb.AppendLine($"{Helper.Tabs(1)}SET @New{EntityName}Id = SCOPE_IDENTITY();");
            sb.AppendLine($"{Helper.Tabs(1)}SET @New{EntityName}{Helper.Capitalize(ColumnToCheck.ColumnName)} = SCOPE_IDENTITY();");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private static string GenerateReadAllProcedure(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE SP_GetAll{tableName}");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"{Helper.Tabs(1)}select * from {tableName};");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private static string GenerateReadProcedure(string tableName, List<ColumnSchema> columns)
        {
            string EntityName = tableName.TrimEnd('s');
            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine($"CREATE PROCEDURE SP_Get{EntityName}ById");
            sb.AppendLine($"CREATE PROCEDURE SP_Get{EntityName}By{Helper.Capitalize(ColumnToCheck.ColumnName)}");
            //sb.AppendLine($"{Helper.Tabs(1)}@Id int");
            sb.AppendLine($"{Helper.Tabs(1)}@{ColumnToCheck.ColumnName} {ColumnToCheck.DataType}");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"{Helper.Tabs(1)}select * from {tableName} where {ColumnToCheck.ColumnName} = @{ColumnToCheck.ColumnName};");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private static string GenerateUpdateProcedure(string tableName, List<ColumnSchema> columns)
        {
            string EntityName = tableName.TrimEnd('s');
            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE SP_Update{EntityName}");
            //sb.AppendLine($"{Helper.Tabs(1)}@Id int,");
            sb.AppendLine($"{Helper.Tabs(1)}@{ColumnToCheck.ColumnName} {ColumnToCheck.DataType},");
            foreach (var column in columns)
            {
                if (column != ColumnToCheck)
                {
                    if (columns.Last() == column)
                    {
                        sb.AppendLine($"{Helper.Tabs(1)}@{column.ColumnName} {column.DataType}");
                    }
                    else
                    {
                        sb.AppendLine($"{Helper.Tabs(1)}@{column.ColumnName} {column.DataType},");
                    }
                }

            }
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"{Helper.Tabs(1)}update {tableName}");
            sb.AppendLine($"{Helper.Tabs(1)}set");
            sb.AppendLine($"{GenerateStoredProcedureAssignments(tableName, columns)}");
            sb.AppendLine($"{Helper.Tabs(1)}where {ColumnToCheck.ColumnName} = @{ColumnToCheck.ColumnName}");
            sb.AppendLine("END");
            return sb.ToString();
        }

        private static string GenerateDeleteProcedure(string tableName, List<ColumnSchema> columns)
        {
            string EntityName = tableName.TrimEnd('s');
            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE PROCEDURE SP_Delete{EntityName}");
            //sb.AppendLine($"{Helper.Tabs(1)}@Id int");
            sb.AppendLine($"{Helper.Tabs(1)}@{ColumnToCheck.ColumnName} {ColumnToCheck.DataType}");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            //sb.AppendLine($"{Helper.Tabs(1)}delete from {tableName} where Id = @Id");
            sb.AppendLine($"{Helper.Tabs(1)}delete from {tableName} where {ColumnToCheck.ColumnName} = @{ColumnToCheck.ColumnName}");
            sb.AppendLine("END");
            return sb.ToString();
        }

        public static void Generate(string DatabaseName)
        {
            Global.ConnectionString = $"Server=.;Database={DatabaseName};User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

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
                    List<ColumnSchema> columns = DatabaseHelper.GetTableSchemaForSql(Global.ConnectionString, tableName);
                    // Create The Controller Class
                    GenerateAndExecuteCRUDProcedures(tableName, columns);
                }
            }

        }

    }
}
