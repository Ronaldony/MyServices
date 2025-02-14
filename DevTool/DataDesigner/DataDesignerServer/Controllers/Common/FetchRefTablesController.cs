using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Common;
    using DataDesignerServer.Core.Types;

    public class FetchRefTablesController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public FetchRefTablesController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpGet]
        [Route("/common/fetch-ref-tables")]
        public RES_FetchRefTables Process()
        {
            var tables = _classDataManager.GetClassTypes(Type_ClassData.Table).Select(d => d.Name).ToList();

            return new RES_FetchRefTables
            {
                Tables = tables,
            };
        }
    }
}
