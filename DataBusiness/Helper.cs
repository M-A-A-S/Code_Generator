using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness
{
    public class Helper
    {
        public static string Tabs(int NumberOfTabs)
        {
            string tabs = "";
            for (int i = 0; i < NumberOfTabs; i++)
            {
                tabs += '\t';
            }
            return tabs;
        }

        public static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static ColumnSchema GetColumnToCheck(string tableName, string ConnectionString)
        {
            List<ColumnSchema> columns = GetTableColumns(tableName);

            ColumnSchema ColumnToCheck;

            bool hasPrimaryKey = DatabaseHelper.HasPrimaryKey(tableName, ConnectionString);

            if (hasPrimaryKey)
            {
                ColumnToCheck = DatabaseHelper.GetPrimaryKeyForTable(ConnectionString, tableName);
            }

            ColumnToCheck = columns[0];

            return ColumnToCheck;
        }

        public static string GetSqlDbType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "int":
                case "int32":
                    return "SqlDbType.Int";

                case "short":
                case "int16":
                    return "SqlDbType.SmallInt";

                case "long":
                case "int64":
                    return "SqlDbType.BigInt";

                case "byte":
                    return "SqlDbType.TinyInt";

                case "bool":
                case "boolean":
                    return "SqlDbType.Bit";

                case "char":
                    return "SqlDbType.Char";

                case "string":
                    return "SqlDbType.NVarChar";

                case "float":
                    return "SqlDbType.Real";

                case "double":
                    return "SqlDbType.Float";

                case "decimal":
                    return "SqlDbType.Decimal";

                case "datetime":
                case "datetime2":
                    return "SqlDbType.DateTime2";

                case "smalldatetime":
                    return "SqlDbType.SmallDateTime";

                case "date":
                    return "SqlDbType.Date";

                case "time":
                    return "SqlDbType.Time";

                case "guid":
                    return "SqlDbType.UniqueIdentifier";

                case "byte[]":
                case "binary":
                    return "SqlDbType.VarBinary";

                case "xml":
                    return "SqlDbType.Xml";

                case "text":
                    return "SqlDbType.Text";

                case "ntext":
                    return "SqlDbType.NText";

                case "varchar":
                    return "SqlDbType.VarChar";

                case "nvarchar":
                    return "SqlDbType.NVarChar";

                case "money":
                    return "SqlDbType.Money";

                case "smallmoney":
                    return "SqlDbType.SmallMoney";

                default:
                    return "SqlDbType.Variant"; // Fallback for unknown types
            }
        }

        public static List<ColumnSchema> GetTableColumns(string tableName)
        {
            return DatabaseHelper.GetTableSchemaForCSharp(Global.ConnectionString, tableName);
        }

    }
}
