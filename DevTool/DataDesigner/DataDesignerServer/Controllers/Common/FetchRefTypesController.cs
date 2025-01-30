using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.TableData;

    public class FetchRefTypesController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        public FetchRefTypesController(
            TableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        [HttpGet]
        [Route("/common/fetch-ref-tables")]
        public List<string> Process()
        {
            var refTables = new List<string>();
            refTables.Add("default");

            //return _tableDataManager.GetSchemaNames().ToList();
            return refTables;
        }
    }
}
