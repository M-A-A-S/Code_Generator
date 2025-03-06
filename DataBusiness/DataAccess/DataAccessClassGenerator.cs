using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.DataAccess
{
    public class DataAccessClassGenerator
    {    

        private static string GenerateReaderParams(List<ColumnSchema> columns)
        {
            string readerParams = "";
            foreach (var column in columns)
            {
                switch (column.DataType)
                {
                    case "string":
                        readerParams += $"{Helper.Tabs(9)}reader.GetString(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "int":
                        readerParams += $"{Helper.Tabs(9)}reader.GetInt32(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "DateTime":
                        readerParams += $"{Helper.Tabs(9)}reader.GetDateTime(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "decimal":
                        readerParams += $"{Helper.Tabs(9)}reader.GetDecimal(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "bool":
                        readerParams += $"{Helper.Tabs(9)}reader.GetBoolean(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "double":
                        readerParams += $"{Helper.Tabs(9)}reader.GetDouble(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "float":
                        readerParams += $"{Helper.Tabs(9)}reader.GetFloat(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "short":
                        readerParams += $"{Helper.Tabs(9)}reader.GetInt16(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "long":
                        readerParams += $"{Helper.Tabs(9)}reader.GetInt64(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "TimeSpan":
                        readerParams += $"{Helper.Tabs(9)}reader.GetTimeSpan(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "byte":
                        readerParams += $"{Helper.Tabs(9)}reader.GetByte(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    case "byte[]":
                        readerParams += $"{Helper.Tabs(9)}(byte[])reader[\"{column.ColumnName}\"],\n";
                        break;
                    case "Guid":
                        readerParams += $"{Helper.Tabs(9)}reader.GetGuid(reader.GetOrdinal(\"{column.ColumnName}\")),\n";
                        break;
                    default:
                        readerParams += $"{Helper.Tabs(9)}reader[\"{column.ColumnName}\"],\n";
                        break;
                }
            }
            readerParams = readerParams.TrimEnd(',', '\n');
            return readerParams;
        }

        private static string GenerateDataAccessCreateMethodSqlParams(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);
            string EntityName = tableName.TrimEnd('s');
            //ColumnSchema ColumnToCheck = GetColumnToCheck(EntityName + "s", columns);
            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            string sqlParams = "";
            foreach (var column in columns)
            {
                if (column.ColumnName != ColumnToCheck.ColumnName)
                {
                    if (columns.Last() == column)
                    {
                        sqlParams += $"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{column.ColumnName}\", {EntityName.ToLower()}DTO.{column.ColumnName});";
                    }
                    else
                    {
                        sqlParams += $"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{column.ColumnName}\", {EntityName.ToLower()}DTO.{column.ColumnName});\n";
                    }
                }
            }
            return sqlParams;
        }

        private static string GenerateDataAccessUpdateMethodSqlParams(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);
            string EntityName = tableName.TrimEnd('s');

            string sqlParams = "";
            foreach (var column in columns)
            {
                if (columns.Last() == column)
                {
                    sqlParams += $"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{column.ColumnName}\", {EntityName.ToLower()}DTO.{column.ColumnName});";
                }
                else
                {
                    sqlParams += $"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{column.ColumnName}\", {EntityName.ToLower()}DTO.{column.ColumnName});\n";
                }
            }
            return sqlParams;
        }


        static void GenerateDataAccessCreateMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string storedProcedureInsert = "SP_AddNew" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}public static int Add{EntityName}({DTOName} {EntityName.ToLower()}DTO)");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add
            //writer.WriteLine($"{Helper.Tabs(3)}int {EntityName.ToLower()}Id = -1;");
            writer.WriteLine($"{Helper.Tabs(3)}{ColumnToCheck.DataType} {ColumnToCheck.ColumnName} = -1;");
            writer.WriteLine($"{Helper.Tabs(3)}try");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Try
            writer.WriteLine($"{Helper.Tabs(4)}using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))");
            writer.WriteLine($"{Helper.Tabs(4)}{{"); // Start SqlConnection 
            writer.WriteLine($"{Helper.Tabs(5)}using (SqlCommand command = new SqlCommand(\"{storedProcedureInsert}\", connection))");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start SqlCommand
            writer.WriteLine($"{Helper.Tabs(6)}command.CommandType = CommandType.StoredProcedure;");
            writer.WriteLine(GenerateDataAccessCreateMethodSqlParams(tableName));
            //writer.WriteLine($"{Helper.Tabs(6)}var outputIdParam = new SqlParameter(\"@New{EntityName}Id\", SqlDbType.Int)");
            writer.WriteLine($"{Helper.Tabs(6)}var outputIdParam = new SqlParameter(\"@New{EntityName}{Helper.Capitalize(ColumnToCheck.ColumnName)}\", {Helper.GetSqlDbType(ColumnToCheck.DataType)})");
            writer.WriteLine($"{Helper.Tabs(6)}{{"); // Start SqlParameter
            writer.WriteLine($"{Helper.Tabs(7)}Direction = ParameterDirection.Output,");
            writer.WriteLine($"{Helper.Tabs(6)}}};"); // End SqlParameter
            writer.WriteLine($"{Helper.Tabs(6)}command.Parameters.Add(outputIdParam);");
            writer.WriteLine($"{Helper.Tabs(6)}connection.Open();");
            writer.WriteLine($"{Helper.Tabs(6)}command.ExecuteNonQuery();");
            //writer.WriteLine($"{Helper.Tabs(6)}{EntityName.ToLower()}Id = (int)outputIdParam.Value;");
            writer.WriteLine($"{Helper.Tabs(6)}{ColumnToCheck.ColumnName} = ({ColumnToCheck.DataType})outputIdParam.Value;");
            //writer.WriteLine($"{Helper.Tabs(6)}return {EntityName.ToLower()}Id;");
            writer.WriteLine($"{Helper.Tabs(6)}return {ColumnToCheck.ColumnName};");
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End SqlCommand
            writer.WriteLine($"{Helper.Tabs(4)}}}"); // End SqlConnection
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Try
            writer.WriteLine($"{Helper.Tabs(3)}catch (Exception ex)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Catch
            writer.WriteLine($"{Helper.Tabs(4)}{"//Console.WriteLine(\"Error\" + ex.Message);"}");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Catch
            //writer.WriteLine($"{Helper.Tabs(3)}return {EntityName.ToLower()}Id;");
            writer.WriteLine($"{Helper.Tabs(3)}return {ColumnToCheck.ColumnName};");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add
        }

        static void GenerateDataAccessReadAllMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string storedProcedureGetAll = "SP_GetAll" + EntityName + 's';

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            writer.WriteLine($"{Helper.Tabs(2)}public static List<{DTOName}> GetAll{EntityName}s()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get All 
            writer.WriteLine($"{Helper.Tabs(3)}var {EntityName}sList = new List<{DTOName}>();");
            writer.WriteLine($"{Helper.Tabs(3)}try");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Try
            writer.WriteLine($"{Helper.Tabs(4)}using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))");
            writer.WriteLine($"{Helper.Tabs(4)}{{"); // Start SqlConnection 
            writer.WriteLine($"{Helper.Tabs(5)}using (SqlCommand command = new SqlCommand(\"{storedProcedureGetAll}\", connection))");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start SqlCommand
            writer.WriteLine($"{Helper.Tabs(6)}command.CommandType = CommandType.StoredProcedure;");
            writer.WriteLine($"{Helper.Tabs(6)}connection.Open();");
            writer.WriteLine($"{Helper.Tabs(6)}using (SqlDataReader reader = command.ExecuteReader())");
            writer.WriteLine($"{Helper.Tabs(6)}{{"); // Start SqlDataReader
            writer.WriteLine($"{Helper.Tabs(7)}while (reader.Read())");
            writer.WriteLine($"{Helper.Tabs(7)}{{"); // Start while
            writer.WriteLine($"{Helper.Tabs(8)}{EntityName}sList.Add(new {DTOName}");
            writer.WriteLine($"{Helper.Tabs(8)}("); // Start reader
            writer.WriteLine(GenerateReaderParams(columns));
            writer.WriteLine($"{Helper.Tabs(8)}));"); // End reader
            writer.WriteLine($"{Helper.Tabs(7)}}}"); // End while
            writer.WriteLine($"{Helper.Tabs(6)}}}"); // End SqlDataReader
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End SqlCommand
            writer.WriteLine($"{Helper.Tabs(4)}}}"); // End SqlConnection
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Try
            writer.WriteLine($"{Helper.Tabs(3)}catch (Exception ex)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Catch
            writer.WriteLine($"{Helper.Tabs(4)}{"//Console.WriteLine(\"Error\" + ex.Message);"}");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Catch
            writer.WriteLine($"{Helper.Tabs(3)}return {EntityName}sList;");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get All
        }

        static void GenerateDataAccessReadMethod(StreamWriter writer, string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);
            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string storedProcedureGetById = "SP_Get" + EntityName + "By" + Helper.Capitalize(ColumnToCheck.ColumnName);



            //writer.WriteLine($"{Helper.Tabs(2)}public static {DTOName} Get{EntityName}ById(int id)");
            writer.WriteLine($"{Helper.Tabs(2)}public static {DTOName} Get{EntityName}By{Helper.Capitalize(ColumnToCheck.ColumnName)}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get By Id 
            writer.WriteLine($"{Helper.Tabs(3)}try");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Try
            writer.WriteLine($"{Helper.Tabs(4)}using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))");
            writer.WriteLine($"{Helper.Tabs(4)}{{"); // Start SqlConnection 
            writer.WriteLine($"{Helper.Tabs(5)}using (SqlCommand command = new SqlCommand(\"{storedProcedureGetById}\", connection))");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start SqlCommand
            writer.WriteLine($"{Helper.Tabs(6)}command.CommandType = CommandType.StoredProcedure;");
            //writer.WriteLine($"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@Id\", id);");
            writer.WriteLine($"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{ColumnToCheck.ColumnName}\", {ColumnToCheck.ColumnName});");
            writer.WriteLine($"{Helper.Tabs(6)}connection.Open();");
            writer.WriteLine($"{Helper.Tabs(6)}using (SqlDataReader reader = command.ExecuteReader())");
            writer.WriteLine($"{Helper.Tabs(6)}{{"); // Start SqlDataReader
            writer.WriteLine($"{Helper.Tabs(7)}if (reader.Read())");
            writer.WriteLine($"{Helper.Tabs(7)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(8)}return new {DTOName}");
            writer.WriteLine($"{Helper.Tabs(8)}("); // Start reader
            writer.WriteLine(GenerateReaderParams(columns));
            writer.WriteLine($"{Helper.Tabs(8)});"); // End reader
            writer.WriteLine($"{Helper.Tabs(7)}}}"); // End if
            writer.WriteLine($"{Helper.Tabs(7)}else");
            writer.WriteLine($"{Helper.Tabs(7)}{{"); // Start else
            writer.WriteLine($"{Helper.Tabs(8)}return null;"); // Start else
            writer.WriteLine($"{Helper.Tabs(7)}}}"); // End else
            writer.WriteLine($"{Helper.Tabs(6)}}}"); // End SqlDataReader
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End SqlCommand
            writer.WriteLine($"{Helper.Tabs(4)}}}"); // End SqlConnection
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Try
            writer.WriteLine($"{Helper.Tabs(3)}catch (Exception ex)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Catch
            writer.WriteLine($"{Helper.Tabs(4)}{"//Console.WriteLine(\"Error\" + ex.Message);"}");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Catch
            writer.WriteLine($"{Helper.Tabs(3)}return null;");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get By Id
        }

        static void GenerateDataAccessUpdateMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string storedProcedureUpdate = "SP_Update" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}public static bool Update{EntityName}({DTOName} {EntityName.ToLower()}DTO)");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Update
            writer.WriteLine($"{Helper.Tabs(3)}int rowsAffected = 0;");
            writer.WriteLine($"{Helper.Tabs(3)}try");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Try
            writer.WriteLine($"{Helper.Tabs(4)}using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))");
            writer.WriteLine($"{Helper.Tabs(4)}{{"); // Start SqlConnection 
            writer.WriteLine($"{Helper.Tabs(5)}using (SqlCommand command = new SqlCommand(\"{storedProcedureUpdate}\", connection))");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start SqlCommand
            writer.WriteLine($"{Helper.Tabs(6)}command.CommandType = CommandType.StoredProcedure;");
            writer.WriteLine(GenerateDataAccessUpdateMethodSqlParams(tableName));
            writer.WriteLine($"{Helper.Tabs(6)}connection.Open();");
            writer.WriteLine($"{Helper.Tabs(6)}rowsAffected = command.ExecuteNonQuery();");
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End SqlCommand
            writer.WriteLine($"{Helper.Tabs(4)}}}"); // End SqlConnection
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Try
            writer.WriteLine($"{Helper.Tabs(3)}catch (Exception ex)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Catch
            writer.WriteLine($"{Helper.Tabs(4)}{"//Console.WriteLine(\"Error\" + ex.Message);"}");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Catch
            writer.WriteLine($"{Helper.Tabs(3)}return (rowsAffected > 0);");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Update
        }

        static void GenerateDataAccessDeleteMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string storedProcedureDelete = "SP_Delete" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            //writer.WriteLine($"{Helper.Tabs(2)}public static bool Delete{EntityName}(int id)");
            writer.WriteLine($"{Helper.Tabs(2)}public static bool Delete{EntityName}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Delete
            writer.WriteLine($"{Helper.Tabs(3)}int rowsAffected = 0;");
            writer.WriteLine($"{Helper.Tabs(3)}try");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Try
            writer.WriteLine($"{Helper.Tabs(4)}using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))");
            writer.WriteLine($"{Helper.Tabs(4)}{{"); // Start SqlConnection 
            writer.WriteLine($"{Helper.Tabs(5)}using (SqlCommand command = new SqlCommand(\"{storedProcedureDelete}\", connection))");
            writer.WriteLine($"{Helper.Tabs(5)}{{"); // Start SqlCommand
            writer.WriteLine($"{Helper.Tabs(6)}command.CommandType = CommandType.StoredProcedure;");
            //writer.WriteLine($"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@Id\", id);");
            writer.WriteLine($"{Helper.Tabs(6)}command.Parameters.AddWithValue(\"@{ColumnToCheck.ColumnName}\", {ColumnToCheck.ColumnName});");
            writer.WriteLine($"{Helper.Tabs(6)}connection.Open();");
            writer.WriteLine($"{Helper.Tabs(6)}rowsAffected = command.ExecuteNonQuery();");
            writer.WriteLine($"{Helper.Tabs(6)}return (rowsAffected == 1);");
            writer.WriteLine($"{Helper.Tabs(5)}}}"); // End SqlCommand
            writer.WriteLine($"{Helper.Tabs(4)}}}"); // End SqlConnection
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Try
            writer.WriteLine($"{Helper.Tabs(3)}catch (Exception ex)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start Catch
            writer.WriteLine($"{Helper.Tabs(4)}{"//Console.WriteLine(\"Error\" + ex.Message);"}");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End Catch
            writer.WriteLine($"{Helper.Tabs(3)}return (rowsAffected > 0);");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Delete
        }

        public static void GenerateDataAccessClass(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName + "Data";
            string DTOName = EntityName + "DTO";

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            writer.WriteLine($"\tpublic class {ClassName}");
            writer.WriteLine("\t{"); // start Data access class


            // Get All 
            writer.WriteLine();
            GenerateDataAccessReadAllMethod(writer, tableName);

            // Get By Id
            writer.WriteLine();
            GenerateDataAccessReadMethod(writer, tableName);

            // Add
            writer.WriteLine();
            GenerateDataAccessCreateMethod(writer, tableName);

            // Update
            writer.WriteLine();
            GenerateDataAccessUpdateMethod(writer, tableName);

            // Delete
            writer.WriteLine();
            GenerateDataAccessDeleteMethod(writer, tableName);

            writer.WriteLine();
            writer.WriteLine("\t}"); // End Data access class
        }
    }
}
