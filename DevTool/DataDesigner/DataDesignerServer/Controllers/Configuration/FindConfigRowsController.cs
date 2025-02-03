
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

    public class FindConfigRowsController : Controller
    {
        private readonly ILogger<FindConfigRowsController> _logger;

        private readonly ConfigDataManager _configDataManager;

        public FindConfigRowsController(
            ILogger<FindConfigRowsController> logger,
            ConfigDataManager configDataManager)
        {
            _logger = logger;

            _configDataManager = configDataManager;
        }

        [HttpGet]
        [Route("/config/find-rows")]
        public RES_FindConfigRows Process(string schema)
        {
            // check schema.
            if (false == _configDataManager.IsExistSchema(schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // rows.
            var rows = _configDataManager.FindRowsOrDefault(schema);

            return new RES_FindConfigRows
            {
                Schema = schema,
                Rows = rows.ToList(),
            };
        }
    }
}
