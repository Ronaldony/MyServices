
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

	/// <summary>
	/// UpdateSchemaController.
	/// </summary>
	public class UpdateSchemaController : Controller
    {
        private readonly ILogger<UpdateSchemaController> _logger;

        private readonly ConfigDataManager _configDataManager;

        public UpdateSchemaController(
            ILogger<UpdateSchemaController> logger,
            ConfigDataManager configDataManager)
        {
            _logger = logger;

            _configDataManager = configDataManager;
        }

        [HttpPost]
        [Route("/config/update-schema")]
        public RES_UpdateConfigSchema Process([FromBody] REQ_UpdateConfigSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (false == _configDataManager.IsExistSchema(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_AlreadyExistSchema, "not exist schema");
            }

            // update schema    
            if (false == _configDataManager.UpdateSchema(req.Schema.Name, req.Schema))
            {
				throw new ServerException(ServerErrorCode.ConfigData_AlreadyExistSchema, "not exist schema");
			}

            return new RES_UpdateConfigSchema();
        }
    }
}
