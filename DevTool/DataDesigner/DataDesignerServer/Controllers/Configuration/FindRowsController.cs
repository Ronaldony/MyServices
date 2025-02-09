
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
    using DataDesignerServer.Core.Http.Configuration;
    using DataDesignerServer.Core.Services.Interfaces;

    public class FindRowsController : Controller
    {
        private readonly ConfigDataManager _configDataManager;

        private readonly IJsonSerializer _jsonSerializer;

        public FindRowsController(
            ConfigDataManager configDataManager,
            IJsonSerializer jsonSerializer)
        {
            _configDataManager = configDataManager;
            _jsonSerializer = jsonSerializer;

        }

        [HttpGet]
        [Route("/config/find-rows")]
        public RES_FindConfigRows Process(string schema)
        {
            // check schema.
            var findSchema = _configDataManager.FindSchemaOrDefault(schema);
            if (default == findSchema)
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

            // rows.
            var oriRows = _configDataManager.FindRowsOrDefault(schema);
            if (default == oriRows)
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistRow, "not exist row even one");
            }

            var rows = ConvertRows(oriRows);

            return new RES_FindConfigRows
            {
                Schema = findSchema,
                Rows = rows.ToList(),
            };
        }

        /// <summary>
        /// Convert rows to new rows which added internal id.
        /// </summary>
        private List<object> ConvertRows(IEnumerable<object> oriRows)
        {
            var dstRows = new List<object>();

            foreach (var pair in oriRows.Select((value, index) => (value, index)))
            {
                var oriRow = pair.value;
                var index = pair.index;

                var rowJson = _jsonSerializer.Serialize(oriRow);
                var dstValue = _jsonSerializer.Deserialize<Dictionary<string, object>>(rowJson);

                dstValue.Add("internalId", Guid.NewGuid());

                dstRows.Add(dstValue);
            }

            return dstRows;
        }
    }
}
