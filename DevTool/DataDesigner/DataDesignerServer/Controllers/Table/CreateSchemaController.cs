
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Table;
    using DataDesignerServer.Core.Types;

    /// <summary>
    /// CreateSchemaController.
    /// </summary>
    public class CreateSchemaController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public CreateSchemaController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpPost]
        [Route("/table/create-schema")]
        public RES_CreateTableSchema Process([FromBody] REQ_CreateTableSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TableData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default != _classDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TableData_AlreadyExistSchema, "already exist schema");
            }

			// Check columns
			foreach (var pair in req.Schema.Columns.Select((value, index) => (value, index)))
            {
                var column = pair.value;
                var index = pair.index;

                if (string.IsNullOrEmpty(column.Name))
                {
                    throw new ServerException(ServerErrorCode.TableData_NoColumnName, $"no column name. idx:{index}");
                }

                if (false == string.IsNullOrEmpty(column.RefTable))
                {
                    column.Type = "int";
                }

                // Set index
                column.Idx = index + 1;
            }

            // Create.
            if (false == _classDataManager.CreateSchema(req.Schema.Name, Type_ClassData.Table, req.Schema))
            {
                throw new ServerException(ServerErrorCode.TableData_CreateSchema, "not exist configuration");
            }

            return new RES_CreateTableSchema();
        }
    }
}
