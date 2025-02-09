
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Type;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// FindSchemaController.
    /// </summary>
    public class FindRowsController : Controller
    {
        private readonly IJsonSerializer _jsonSerializer;

        private readonly TypeDataManager _typeDataManager;

        public FindRowsController(
            IJsonSerializer jsonSerializer,
            TypeDataManager typeDataManager)
        {
            _jsonSerializer = jsonSerializer;
            _typeDataManager = typeDataManager;
        }

        [HttpGet]
        [Route("/type/find-rows")]
        public RES_FindTypeRows Process(string schema)
        {
            // find schema.
            var findSchema = _typeDataManager.FindSchemaOrDefault(schema);
            if (null == findSchema)
            {
                throw new ServerException(ServerErrorCode.TypeData_NotExistSchema, "not exist schema");
            }

            // find rows.
            var findRows = _typeDataManager.GetRowsOrDefault(schema);
            if (null == findSchema)
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistRow, "not exist schema even one");
            }

            var rows = ConvertRows(findRows);

            return new RES_FindTypeRows
            {
                Schema = findSchema,
                Rows = rows,
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
