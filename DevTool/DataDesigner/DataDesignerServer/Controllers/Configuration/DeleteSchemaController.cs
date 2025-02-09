
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

	/// <summary>
	/// DeleteSchemaController.
	/// </summary>
	public class DeleteSchemaController : Controller
    {
        private readonly ConfigDataManager _configDataManager;

        public DeleteSchemaController(
            ConfigDataManager configDataManager)
        {
            _configDataManager = configDataManager;
        }

        [HttpDelete]
        [Route("/config/delete-schema")]
        public RES_DeleteConfigSchema Process([FromBody] REQ_DeleteConfigSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _configDataManager.FindSchemaOrDefault(req.Schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // delete schema.
            if (false == _configDataManager.DeleteSchema(req.Schema))
            {
				throw new ServerException(ServerErrorCode.ConfigData_DeleteSchema, "not exist schema");
			}

            return new RES_DeleteConfigSchema();
        }
    }
}
