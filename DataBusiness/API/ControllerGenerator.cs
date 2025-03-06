using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBusiness.API
{
    public class ControllerGenerator
    {
       

        //private static string GenerateDTOConstructorArguments(List<ColumnSchema> columns, string DTOName)
        private static string GenerateDTOConstructorArguments(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";


            string DTOConstructorArgs = "";
            foreach (var column in columns)
            {
                DTOConstructorArgs += $"new{DTOName}.{column.ColumnName}, ";
            }
            DTOConstructorArgs = DTOConstructorArgs.TrimEnd(',', ' ');
            return DTOConstructorArgs;
        }

        //private static string GenerateAssignmentUpdate(List<ColumnSchema> columns, string EntityName)
        private static string GenerateAssignmentUpdate(string tableName)
        {
            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";

            string AssignmentUpdate = "";

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            foreach (var column in columns)
            {
                if (column.ColumnName != ColumnToCheck.ColumnName)
                {
                    if (columns.Last() == column)
                    {
                        AssignmentUpdate += $"{Helper.Tabs(3)}{EntityName}.{column.ColumnName} = updated{EntityName}DTO.{column.ColumnName};";
                    }
                    else
                    {
                        AssignmentUpdate += $"{Helper.Tabs(3)}{EntityName}.{column.ColumnName} = updated{EntityName}DTO.{column.ColumnName};\n";
                    }
                }

            }
            return AssignmentUpdate;
        }

        public static void GenerateValidationForAdd(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(3)}if");
            writer.WriteLine($"{Helper.Tabs(3)}(");
            writer.WriteLine($"{Helper.Tabs(4)}new{DTOName} == null");
            foreach (var column in columns)
            {
                if (ColumnToCheck.ColumnName != column.ColumnName)
                {
                    if (column.DataType == "string")
                    {
                        writer.WriteLine($"{Helper.Tabs(4)}|| string.IsNullOrEmpty(new{DTOName}.{column.ColumnName})");
                    }
                    else if (column.DataType == "int" || column.DataType == "long" || column.DataType == "short" || column.DataType == "byte")
                    {
                        writer.WriteLine($"{Helper.Tabs(4)}|| new{DTOName}.{column.ColumnName} < 1");
                    }
                }
            }
            writer.WriteLine($"{Helper.Tabs(3)})");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return BadRequest(\"Invalid {EntityName.ToLower()} data.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
        }

        public static void GenerateValidationForUpdate(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(3)}if");
            writer.WriteLine($"{Helper.Tabs(3)}(");
            //writer.WriteLine($"{Helper.Tabs(4)}Id < 1");
            writer.WriteLine($"{Helper.Tabs(4)}{ColumnToCheck.ColumnName} < 1");
            writer.WriteLine($"{Helper.Tabs(4)}|| updated{DTOName} == null");
            foreach (var column in columns)
            {
                if (ColumnToCheck.ColumnName != column.ColumnName)
                {
                    if (column.DataType == "string")
                    {
                        writer.WriteLine($"{Helper.Tabs(4)}|| string.IsNullOrEmpty(updated{DTOName}.{column.ColumnName})");
                    }
                    else if (column.DataType == "int" || column.DataType == "long" || column.DataType == "short" || column.DataType == "byte")
                    {
                        writer.WriteLine($"{Helper.Tabs(4)}|| updated{DTOName}.{column.ColumnName} < 1");
                    }
                }

            }
            writer.WriteLine($"{Helper.Tabs(3)})");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return BadRequest(\"Invalid {EntityName.ToLower()} data.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
        }

        static void GenerateCreateMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            writer.WriteLine($"{Helper.Tabs(2)}[HttpPost(Name = \"Add{EntityName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status201Created)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
            writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<{DTOName}> Add{EntityName}({DTOName} new{DTOName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add

            GenerateValidationForAdd(writer, tableName);

            writer.WriteLine();

            //writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = new {EntityName}(new {DTOName}({GenerateDTOConstructorArguments(columns, DTOName)}));");

            writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = new cls{EntityName}(new {DTOName}({GenerateDTOConstructorArguments(tableName)}));");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if (!{EntityName}.Save())");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return StatusCode(StatusCodes.Status500InternalServerError, \"An error occurred while processing the request.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            //writer.WriteLine($"{Helper.Tabs(3)}new{DTOName}.Id = {EntityName}.Id;");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}new{DTOName}.{ColumnToCheck.ColumnName} = {EntityName}.{ColumnToCheck.ColumnName};");
            writer.WriteLine($"{Helper.Tabs(3)}//we don't return Ok here,we return createdAtRoute: this will be status code 201 created.");
            //writer.WriteLine($"{Helper.Tabs(3)}return CreatedAtRoute(\"Get{EntityName}ById\", new {{ id = new{DTOName}.Id }}, new{DTOName});");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}return CreatedAtRoute(\"Get{EntityName}ById\", new {{ {ColumnToCheck.ColumnName} = new{DTOName}.{ColumnToCheck.ColumnName} }}, new{DTOName});");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add

        }

        static void GenerateReadAllMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            writer.WriteLine($"{Helper.Tabs(2)}[HttpGet(\"All\", Name = \"GetAll{EntityName}s\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status200OK)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status404NotFound)]");
            writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<IEnumerable<{DTOName}>> GetAll{EntityName}s()");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get All
            writer.WriteLine($"{Helper.Tabs(3)}List<{DTOName}> {EntityName}sList = cls{EntityName}.GetAll{EntityName}s();");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if ({EntityName}sList.Count == 0)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return NotFound(\"No {EntityName}s Found!\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}return Ok({EntityName}sList);");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get All
        }

        static void GenerateReadMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;


            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            //writer.WriteLine($"{Helper.Tabs(2)}[HttpGet(\"{{Id}}\", Name = \"Get{EntityName}ById\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[HttpGet(\"{{{ColumnToCheck.ColumnName}}}\", Name = \"Get{EntityName}By{ColumnToCheck.ColumnName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status200OK)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status404NotFound)]");
            //writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<{DTOName}> Get{EntityName}ById(int Id)");
            writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<{DTOName}> Get{EntityName}By{ColumnToCheck.ColumnName}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Get By Id
            //writer.WriteLine($"{Helper.Tabs(3)}if (Id < 1)");
            writer.WriteLine($"{Helper.Tabs(3)}if ({ColumnToCheck.ColumnName} < 1)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            //writer.WriteLine($"{Helper.Tabs(4)}return BadRequest($\"Not accepted Id {{Id}}\");");
            writer.WriteLine($"{Helper.Tabs(4)}return BadRequest($\"Not accepted {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}}\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine();
            //writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = cls{EntityName}.Find(Id);");

            writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = cls{EntityName}.Find({ColumnToCheck.ColumnName});");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if ({EntityName} == null)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            //writer.WriteLine($"{Helper.Tabs(4)}return NotFound($\"{EntityName} with ID {{Id}} not found\");");
            writer.WriteLine($"{Helper.Tabs(4)}return NotFound($\"{EntityName} with {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}} not found\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine();

            writer.WriteLine($"{Helper.Tabs(3)}{DTOName} {DTOName} = {EntityName}.{DTOName};");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}return Ok({DTOName});");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Get By Id
        }

        static void GenerateUpdateMethod(StreamWriter writer, string tableName)
        {

            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            //writer.WriteLine($"{Helper.Tabs(2)}[HttpPut(\"{{Id}}\", Name = \"Update{EntityName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[HttpPut(\"{{{ColumnToCheck.ColumnName}}}\", Name = \"Update{EntityName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status200OK)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status404NotFound)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
            //writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<{DTOName}> Update{EntityName}(int Id, {DTOName} updated{DTOName})");
            writer.WriteLine($"{Helper.Tabs(2)}public ActionResult<{DTOName}> Update{EntityName}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName}, {DTOName} updated{DTOName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Update

            GenerateValidationForUpdate(writer, tableName);

            writer.WriteLine();
            //writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = cls{EntityName}.Find(Id);");
            writer.WriteLine($"{Helper.Tabs(3)}cls{EntityName} {EntityName} = cls{EntityName}.Find({ColumnToCheck.ColumnName});");
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if ({EntityName} == null)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            //writer.WriteLine($"{Helper.Tabs(4)}return NotFound($\"{EntityName} with ID {{Id}} not found.\");");
            writer.WriteLine($"{Helper.Tabs(4)}return NotFound($\"{EntityName} with {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}} not found.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine();
            writer.WriteLine(GenerateAssignmentUpdate(tableName));
            //writer.WriteLine($"{Helper.Tabs(3)}"); //
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if (!{EntityName}.Save())");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return StatusCode(StatusCodes.Status500InternalServerError, \"An error occurred while processing the request.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}return Ok({EntityName}.{DTOName});");
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Update

        }

        static void GenerateDeleteMethod(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string DTOName = EntityName + "DTO";
            string ClassName = "cls" + EntityName;

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);

            ColumnSchema ColumnToCheck = Helper.GetColumnToCheck(tableName, Global.ConnectionString);

            //writer.WriteLine($"{Helper.Tabs(2)}[HttpDelete(\"{{Id}}\", Name = \"Delete{EntityName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[HttpDelete(\"{{{ColumnToCheck.ColumnName}}}\", Name = \"Delete{EntityName}\")]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status200OK)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            writer.WriteLine($"{Helper.Tabs(2)}[ProducesResponseType(StatusCodes.Status404NotFound)]");
            //writer.WriteLine($"{Helper.Tabs(2)}public ActionResult Delete{EntityName}(int Id)");
            writer.WriteLine($"{Helper.Tabs(2)}public ActionResult Delete{EntityName}({ColumnToCheck.DataType} {ColumnToCheck.ColumnName})");
            writer.WriteLine($"{Helper.Tabs(2)}{{"); // Start Add
            //writer.WriteLine($"{Helper.Tabs(3)}if (Id < 1)");

            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(3)}if ({ColumnToCheck.ColumnName} < 1)");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return BadRequest($\"Not accepted {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}}\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if

            writer.WriteLine();

            writer.WriteLine($"{Helper.Tabs(3)}if (cls{EntityName}.Delete{EntityName}({ColumnToCheck.ColumnName}))");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start if
            writer.WriteLine($"{Helper.Tabs(4)}return Ok($\"{EntityName} with {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}} has been deleted.\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End if
            writer.WriteLine($"{Helper.Tabs(3)}else");
            writer.WriteLine($"{Helper.Tabs(3)}{{"); // Start else
            writer.WriteLine($"{Helper.Tabs(4)}return NotFound($\"{EntityName} with {ColumnToCheck.ColumnName} {{{ColumnToCheck.ColumnName}}} not found. no rows deleted!\");");
            writer.WriteLine($"{Helper.Tabs(3)}}}"); // End else
            writer.WriteLine($"{Helper.Tabs(2)}}}"); // End Add            
        }

        public static void GenerateController(StreamWriter writer, string tableName)
        {
            string EntityName = tableName.TrimEnd('s');
            string ClassName = "cls" + EntityName;
            string DTOName = EntityName + "DTO";

            List<ColumnSchema> columns = Helper.GetTableColumns(tableName);


            writer.WriteLine($"{Helper.Tabs(1)}public class {EntityName}sController : ControllerBase");
            writer.WriteLine($"{Helper.Tabs(1)}{{"); // Start Controller
            //writer.WriteLine();

            //GetPropertySection(writer, tableName, columns);

            //writer.WriteLine();
            //GenerateDataBusinessConstructorSection(writer, tableName, columns);


            // Get All
            writer.WriteLine();
            GenerateReadAllMethod(writer, tableName);
            // Get By Id
            writer.WriteLine();
            GenerateReadMethod(writer, tableName);
            // Add
            writer.WriteLine();
            GenerateCreateMethod(writer, tableName);
            // Update
            writer.WriteLine();
            GenerateUpdateMethod(writer, tableName);
            // DeleteBook
            writer.WriteLine();
            GenerateDeleteMethod(writer, tableName);

            writer.WriteLine();
            writer.WriteLine($"{Helper.Tabs(1)}}}"); // End Controller
        }

    }
}
