using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Type;

    /// <summary>
    /// GetAllTypeController.
    /// </summary>
    public sealed class GetAllTypeController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public GetAllTypeController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpGet]
        [Route("/type/get-all")]
        public RES_GetAllType Process()
        {
            return new RES_GetAllType
            {
                SchemaNames = _typeDataManager.GetSchemaNames().ToList(),
            };
        }
    }
}
