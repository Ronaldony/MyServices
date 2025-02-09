
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

    /// <summary>
    /// FindSchemaController.
    /// </summary>
    public class FindSchemaController : Controller
    {
        private readonly ConfigDataManager _configDataManager;

        public FindSchemaController(
            ConfigDataManager configDataManager)
        {
            _configDataManager = configDataManager;
        }

        [HttpGet]
        [Route("/config/find-schema")]
        public RES_FindConfigSchema Process(string schema)
        {
            var findSchema = _configDataManager.FindSchemaOrDefault(schema);
            if (null == findSchema)
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            return new RES_FindConfigSchema
            {
                Schema = findSchema
            };
        }
    }
}
