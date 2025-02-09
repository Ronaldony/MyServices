
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

    /// <summary>
    /// CreateSchemaController.
    /// </summary>
    public class CreateSchemaController : Controller
    {
        private readonly ConfigDataManager _configDataManager;

        public CreateSchemaController(
            ConfigDataManager configDataManager)
        {
            _configDataManager = configDataManager;
        }

        [HttpPost]
        [Route("/config/create-schema")]
        public RES_CreateConfigSchema Process([FromBody] REQ_CreateConfigSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default != _configDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_AlreadyExistSchema, "already exist schema");
            }

            // Check
            foreach (var pair in req.Schema.Columns.Select((value, index) => (value, index)))
            {
                var column = pair.value;
                var index = pair.index;

                if (string.IsNullOrEmpty(column.Name))
                {
                    throw new ServerException(ServerErrorCode.ConfigData_NoColumnName, $"no column name. idx:{index}");
                }

                // Set index
                column.Idx = index + 1;
            }

            // Create.
            if (false == _configDataManager.CreateSchema(req.Schema.Name, req.Schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_CreateSchema, "not exist configuration");
            }

            return new RES_CreateConfigSchema();
        }
    }
}
