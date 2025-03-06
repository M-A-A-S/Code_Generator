using DataAccess;

namespace DataBusiness
{
    public class DatabaseHelper
    {
        public static bool IsDatabaseExists(string connectionString, string databaseName)
        {
            return DatabaseHelperData.IsDatabaseExists(connectionString, databaseName);
        }

        public static List<string> GetAllDatabasesNames(string connectionString)
        {
            return DatabaseHelperData.GetAllDatabasesNames(connectionString);
        }

        public static List<string> GetTableNamesOfDatabase(string connectionString)
        {
            return DatabaseHelperData.GetTableNamesOfDatabase(connectionString);
        }

        public static List<ColumnSchema> GetTableSchemaForCSharp(string connectionString, string tableName)
        {
            return DatabaseHelperData.GetTableSchemaForCSharp(connectionString, tableName);
        }

        public static List<ColumnSchema> GetTableSchemaForSql(string connectionString, string tableName)
        {
            return DatabaseHelperData.GetTableSchemaForSql(connectionString, tableName);
        }

        public static ColumnSchema GetPrimaryKeyForTable(string connectionString, string tableName)
        {
            return DatabaseHelperData.GetPrimaryKeyForTable(connectionString, tableName);
        }

        public static List<ColumnSchema> GetPrimaryKeysForTable(string connectionString, string tableName)
        {
            return DatabaseHelperData.GetPrimaryKeysForTable(connectionString, tableName);
        }

        public static bool HasPrimaryKey(string tableName, string connectionString)
        {
            return DatabaseHelperData.HasPrimaryKey(tableName, connectionString);
        }

        public static bool ExecuteSqlCommand(string sqlCommand, string ConnectionString)
        {
            return DatabaseHelperData.ExecuteSqlCommand(sqlCommand, ConnectionString);  
        }

    }
}
