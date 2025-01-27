using DataDesignerServer.Core.Data;
using DataDesignerServer.Core.DataManager.ConfigData;
using DataDesignerServer.Core.Http.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    public class CreateConfigSchemaController
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
            var schema = new ClassSchema
            {
                Name = schemaName,
                Category = category,
                Columns = new List<ClassSchemaColumn>(),
                Desciption = desc
            };

            // Create.
            if (false == _configDataManager.CreateSchema(schema.Name, schema))
            {
                return "fail.";
            }

            return new RES_CreateConfigSchema
            {

            };
        }
    }
}
