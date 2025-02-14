using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Configuration;
    using DataDesignerServer.Core.Types;

    public sealed class GetAllConfigController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public GetAllConfigController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpGet]
        [Route("/config/get-all")] 
        public RES_GetAllConfig Process()
        {
            return new RES_GetAllConfig
            {
                SchemaNames = _classDataManager.GetClassTypes(Type_ClassData.Configuration).Select(d => d.Name).ToList(),
            };
        }
    }
}
