
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Table;

    /// <summary>
    /// DeleteSchemaController.
    /// </summary>
    public class DeleteSchemaController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public DeleteSchemaController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpDelete]
        [Route("/table/delete-schema")]
        public RES_DeleteTableSchema Process([FromBody] REQ_DeleteTableSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema))
            {
                throw new ServerException(ServerErrorCode.TableData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _classDataManager.FindSchemaOrDefault(req.Schema))
            {
                throw new ServerException(ServerErrorCode.TableData_NotExistSchema, "not exist schema");
            }

            // delete schema.
            if (false == _classDataManager.DeleteSchema(req.Schema))
            {
				throw new ServerException(ServerErrorCode.TableData_DeleteSchema, "not exist schema");
			}

            return new RES_DeleteTableSchema();
        }
    }
}
