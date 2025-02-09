using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;

    public sealed class GetAllConfigController : Controller
    {
        private readonly ConfigDataManager _configDataManager;

        public GetAllConfigController(
            ConfigDataManager configDataManager)
        {
            _configDataManager = configDataManager;
        }

        [HttpGet]
        [Route("/config/get-all")] 
        public RES_GetAllConfig Process()
        {
            return new RES_GetAllConfig
            {
                SchemaNames = _configDataManager.GetSchemaNames().ToList(),
            };
        }
    }
}
