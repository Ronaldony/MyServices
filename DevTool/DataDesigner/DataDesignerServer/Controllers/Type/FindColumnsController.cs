
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Type;

    /// <summary>
    /// FindColumnsController.
    /// </summary>
    public class FindColumnsController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public FindColumnsController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpGet]
        [Route("/type/find-columns")]
        public RES_FindTypeColumns Process()
        {
            return new RES_FindTypeColumns
            {
                Columns = _typeDataManager.GetSchemaColumns(),
            };
        }
    }
}
