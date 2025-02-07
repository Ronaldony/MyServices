using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Common;

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
        public RES_FetchRefTables Process()
        {
            var tables = _tableDataManager.GetSchemaNames().ToList();

            return new RES_FetchRefTables
            {
                Tables = tables,
            };
        }
    }
}
