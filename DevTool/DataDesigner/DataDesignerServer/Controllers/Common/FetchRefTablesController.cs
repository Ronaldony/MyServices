using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.TableData;

    public class FetchRefTablesController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        public FetchRefTablesController(
            TableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        [HttpGet]
        [Route("/common/fetch-ref-tables")]
        public List<string> Process()
        {
            return _tableDataManager.GetSchemaNames().ToList();
        }
    }
}
