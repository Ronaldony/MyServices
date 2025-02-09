
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Type;

    /// <summary>
    /// DeleteSchemaController.
    /// </summary>
    public class DeleteSchemaController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public DeleteSchemaController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpDelete]
        [Route("/type/delete-schema")]
        public RES_DeleteTypeSchema Process([FromBody] REQ_DeleteTypeSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _typeDataManager.FindSchemaOrDefault(req.Schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // delete schema.
            if (false == _typeDataManager.DeleteSchema(req.Schema))
            {
				throw new ServerException(ServerErrorCode.ConfigData_DeleteSchema, "not exist schema");
			}

            return new RES_DeleteTypeSchema();
        }
    }
}
