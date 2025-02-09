using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.DataManager.TypeData;
	using DataDesignerServer.Core.Http.Common;

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
        public RES_FetchTypes Process()
        {
            var types = _typeDataManager.GetTypeSchemasWithPrimitive().ToList();

            return new RES_FetchTypes
            {
                Types = types,
            };
        }
    }
}
