using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.TypeData;

    public class FetchTypesController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public FetchTypesController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpGet]
        [Route("/common/fetch-types")]
        public List<string> Process()
        {
            return _typeDataManager.GetSchemaNames().ToList();
        }
    }
}
