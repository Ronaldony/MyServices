
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;
	using DataDesignerServer.Core.Services.Interfaces;

	public class FindConfigRowsController : Controller
    {
        private readonly ILogger<FindConfigRowsController> _logger;

        private readonly ConfigDataManager _configDataManager;

        private readonly IJsonSerializer _jsonSerializer;

        public FindConfigRowsController(
            ILogger<FindConfigRowsController> logger,
            ConfigDataManager configDataManager,
			IJsonSerializer jsonSerializer)
        {
            _logger = logger;

            _configDataManager = configDataManager;
            _jsonSerializer = jsonSerializer;

		}

        [HttpGet]
        [Route("/config/find-rows")]
        public RES_FindConfigRows Process(string schema)
        {
            // check schema.
            if (false == _configDataManager.IsExistSchema(schema))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // rows.
            var oriRows = _configDataManager.FindRowsOrDefault(schema);

            //var rows = ConvertRows(oriRows);

			return new RES_FindConfigRows
            {
                Schema = schema,
                Rows = oriRows.ToList(),
			};
        }

        /// <summary>
        /// Convert rows to new rows which added internal id.
        /// </summary>
        private List<object> ConvertRows(IEnumerable<object> oriRows)
        {
            var dstRows = new List<object>();

   //         foreach (var oriRow in oriRows)
   //         {
   //             var desValue = _jsonSerializer.Deserialize<Dictionary<string, string>>(oriRow);

   //             desValue.Add("internalId", desValue["Id"]);
			//}

            return default;
        }
    }
}
