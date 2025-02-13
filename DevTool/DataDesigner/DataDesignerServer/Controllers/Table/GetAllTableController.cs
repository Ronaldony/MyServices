using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Table;

	public sealed class GetAllTableController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        public GetAllTableController(
			TableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        [HttpGet]
        [Route("/table/get-all")] 
        public RES_GetAllTable Process()
        {
            return new RES_GetAllTable
			{
                SchemaNames = _tableDataManager.GetAllType().Select(d => d.Name).ToList(),
            };
        }
    }
}
