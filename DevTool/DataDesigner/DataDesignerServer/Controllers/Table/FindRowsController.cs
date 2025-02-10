
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Configuration;
	using DataDesignerServer.Core.Http.Table;
	using DataDesignerServer.Core.Services.Interfaces;

	public class FindRowsController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        private readonly IJsonSerializer _jsonSerializer;

        public FindRowsController(
			TableDataManager tableDataManager,
            IJsonSerializer jsonSerializer)
        {
            _tableDataManager = tableDataManager;
            _jsonSerializer = jsonSerializer;
        }

        [HttpGet]
        [Route("/table/find-rows")]
        public RES_FindTableRows Process(string schema)
        {
            // check schema.
            var findSchema = _tableDataManager.FindSchemaOrDefault(schema);
            if (default == findSchema)
            {
                throw new ServerException(ServerErrorCode.TableData_NotExistSchema, "not exist schema");
            }

            // rows.
            var oriRows = _tableDataManager.FindRowsOrDefault(schema);
            if (default == oriRows)
            {
                throw new ServerException(ServerErrorCode.TableData_NotExistRow, "not exist row even one");
            }

            var rows = ConvertRows(oriRows);

            return new RES_FindTableRows
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
