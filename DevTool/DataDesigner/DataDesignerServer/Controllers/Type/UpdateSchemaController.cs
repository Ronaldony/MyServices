
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Configuration;
    using DataDesignerServer.Core.Http.Type;

    /// <summary>
    /// UpdateSchemaController.
    /// </summary>
    public class UpdateSchemaController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public UpdateSchemaController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpPost]
        [Route("/type/update-schema")]
        public RES_UpdateTypeSchema Process([FromBody] REQ_UpdateTypeSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _typeDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // update schema    
            if (false == _typeDataManager.UpdateSchema(req.Schema.Name, req.Schema))
            {
				throw new ServerException(ServerErrorCode.ConfigData_AlreadyExistSchema, "not exist schema");
			}

            return new RES_UpdateTypeSchema();
        }
    }
}
