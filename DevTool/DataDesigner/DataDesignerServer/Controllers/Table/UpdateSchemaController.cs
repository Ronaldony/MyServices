
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Table;

	/// <summary>
	/// UpdateSchemaController.
	/// </summary>
	public class UpdateSchemaController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        public UpdateSchemaController(
			TableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        [HttpPost]
        [Route("/table/update-schema")]
        public RES_UpdateTableSchema Process([FromBody] REQ_UpdateTableSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TableData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _tableDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TableData_NotExistSchema, "not exist schema");
            }

			// Check columns
			foreach (var column in req.Schema.Columns)
			{
				if (string.IsNullOrEmpty(column.Name))
				{
					throw new ServerException(ServerErrorCode.TableData_NoColumnName, $"no column name.");
				}

				if (false == string.IsNullOrEmpty(column.RefTable))
				{
					column.Type = "int";
				}
			}

			// update schema    
			if (false == _tableDataManager.UpdateSchema(req.Schema.Name, req.Schema))
            {
				throw new ServerException(ServerErrorCode.TableData_UpdateSchema, "fail to update schema");
			}

            return new RES_UpdateTableSchema();
        }
    }
}
