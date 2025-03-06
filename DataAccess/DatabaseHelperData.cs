using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    public class DatabaseHelperData
    {

        public static bool IsDatabaseExists(string connectionString, string databaseName)
        {
            bool exists = false;

            string query = "SELECT COUNT(*) FROM sys.databases WHERE name = @DatabaseName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DatabaseName", databaseName);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    exists = count > 0;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return exists;
        }

        public static List<string> GetAllDatabasesNames(string connectionString)
        {
            List<string> databases = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT name FROM sys.databases";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return databases;
        }

        public static List<string> GetTableNamesOfDatabase(string connectionString)
        {
            List<string> tableNames = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tableNames.Add(reader["TABLE_NAME"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return tableNames;
        }

        public static List<ColumnSchema> GetTableSchemaForCSharp(string connectionString, string tableName)
        {
            List<ColumnSchema> columns = new List<ColumnSchema>();

            string query = $@"
                SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @TableName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableName", tableName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columns.Add(new ColumnSchema
                                {
                                    ColumnName = reader["COLUMN_NAME"].ToString(),
                                    DataType = MapSqlDataTypeToCSharp(reader["DATA_TYPE"].ToString()),
                                    IsNullable = reader["IS_NULLABLE"].ToString() == "YES"
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return columns;
        }

        public static List<ColumnSchema> GetTableSchemaForSql(string connectionString, string tableName)
        {
            List<ColumnSchema> columns = new List<ColumnSchema>();

            string query = $@"
        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, 
               CHARACTER_MAXIMUM_LENGTH 
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = @TableName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableName", tableName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string dataType = reader["DATA_TYPE"].ToString();
                                string columnName = reader["COLUMN_NAME"].ToString();
                                bool isNullable = reader["IS_NULLABLE"].ToString() == "YES";

                                // Append size for string-based types
                                if (dataType == "nvarchar" || dataType == "varchar" ||
                                    dataType == "char" || dataType == "nchar")
                                {
                                    int maxLength = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value
                                        ? Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"])
                                        : -1; // -1 means MAX in SQL Server

                                    dataType += maxLength == -1 ? "(MAX)" : $"({maxLength})";
                                }

                                columns.Add(new ColumnSchema
                                {
                                    ColumnName = columnName,
                                    DataType = dataType,
                                    IsNullable = isNullable
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }            

            return columns;
        }

        public static ColumnSchema GetPrimaryKeyForTable(string connectionString, string tableName)
        {
            ColumnSchema schema = null;

            string query = @"
            SELECT TOP 1 
                k.COLUMN_NAME, 
                c.DATA_TYPE, 
                CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
            JOIN INFORMATION_SCHEMA.COLUMNS c 
                ON k.TABLE_NAME = c.TABLE_NAME 
                AND k.COLUMN_NAME = c.COLUMN_NAME
            WHERE OBJECTPROPERTY(OBJECT_ID(k.CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
            AND k.TABLE_NAME = @TableName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Get primary key info
                        {
                            schema = new ColumnSchema
                            {
                                ColumnName = reader["COLUMN_NAME"].ToString(),
                                DataType = reader["DATA_TYPE"].ToString(),
                                IsNullable = Convert.ToBoolean(reader["IsNullable"])
                            };
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return schema; // Returns null if no primary key is found
        }

        public static List<ColumnSchema> GetPrimaryKeysForTable(string connectionString, string tableName)
        {
            List<ColumnSchema> primaryKeys = new List<ColumnSchema>();

            string query = @"
            SELECT k.COLUMN_NAME, c.DATA_TYPE, 
                   CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
            JOIN INFORMATION_SCHEMA.COLUMNS c 
                ON k.TABLE_NAME = c.TABLE_NAME 
                AND k.COLUMN_NAME = c.COLUMN_NAME
            WHERE OBJECTPROPERTY(OBJECT_ID(k.CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
            AND k.TABLE_NAME = @TableName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Fetch all primary key details
                        {
                            primaryKeys.Add(new ColumnSchema
                            {
                                ColumnName = reader["COLUMN_NAME"].ToString(),
                                DataType = reader["DATA_TYPE"].ToString(),
                                IsNullable = Convert.ToBoolean(reader["IsNullable"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return primaryKeys; // Returns an empty list if no primary key is found
        }

        public static bool HasPrimaryKey(string tableName, string connectionString)
        {
            bool HasPrimaryKey = false;

            string query = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
            WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
            AND TABLE_NAME = @TableName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    HasPrimaryKey = count > 0;
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return HasPrimaryKey;
            
        }

        public static string MapSqlDataTypeToCSharp(string sqlDataType)
        {
            var typeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bigint", "long" },
            { "binary", "byte[]" },
            { "bit", "bool" },
            { "char", "string" },
            { "date", "DateTime" },
            { "datetime", "DateTime" },
            { "datetime2", "DateTime" },
            { "datetimeoffset", "DateTimeOffset" },
            { "decimal", "decimal" },
            { "float", "double" },
            { "image", "byte[]" },
            { "int", "int" },
            { "money", "decimal" },
            { "nchar", "string" },
            { "ntext", "string" },
            { "numeric", "decimal" },
            { "nvarchar", "string" },
            { "real", "float" },
            { "smalldatetime", "DateTime" },
            { "smallint", "short" },
            { "smallmoney", "decimal" },
            { "text", "string" },
            { "time", "TimeSpan" },
            { "timestamp", "byte[]" },
            { "tinyint", "byte" },
            { "uniqueidentifier", "Guid" },
            { "varbinary", "byte[]" },
            { "varchar", "string" },
            { "xml", "string" }
            };

            return typeMapping.TryGetValue(sqlDataType, out string csharpType) ? csharpType : "object";
        }

        public static bool ExecuteSqlCommand(string sqlCommand, string ConnectionString)
        {
            if (string.IsNullOrWhiteSpace(sqlCommand))
            {
                Console.WriteLine("Invalid SQL command.");
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL command executed successfully.");
                        return true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}\n{sqlEx.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}\n{ex.StackTrace}");
            }

            return false;
        }


    }
}
