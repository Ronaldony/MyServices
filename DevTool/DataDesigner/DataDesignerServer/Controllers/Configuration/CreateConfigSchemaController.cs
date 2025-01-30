using DataDesignerServer.Core.Common;
using DataDesignerServer.Core.DataManager.ConfigData;
using DataDesignerServer.Core.Http.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    public class CreateConfigSchemaController : Controller
    {
        private readonly ILogger<CreateConfigSchemaController> _logger;

        private readonly ConfigDataManager _configDataManager;

        public CreateConfigSchemaController(
            ILogger<CreateConfigSchemaController> logger,
            ConfigDataManager configDataManager)
        {
            _logger = logger;

            _configDataManager = configDataManager;
        }

        [HttpPost]
        [Route("/config/create-schema")]
        public RES_CreateConfigSchema Process([FromBody] REQ_CreateConfigSchema req)
        {
            // Create.
            if (false == _configDataManager.CreateSchema(req.Schema.Name, req.Schema))
            {
                throw new ServerException(ServerErrorCode.FailCreateConfigSchema, "not exist configuration");
            }

            return new RES_CreateConfigSchema();
        }
    }
}
