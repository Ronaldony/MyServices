using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Table;
    using DataDesignerServer.Core.Types;

    public sealed class GetAllTableController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public GetAllTableController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpGet]
        [Route("/table/get-all")] 
        public RES_GetAllTable Process()
        {
            return new RES_GetAllTable
			{
                SchemaNames = _classDataManager.GetClassTypes(Type_ClassData.Table).Select(d => d.Name).ToList(),
            };
        }
    }
}
