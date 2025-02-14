
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Configuration;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// UpdateRowsController.
    /// </summary>
    public class UpdateRowsController : Controller
    {
        // internal id key for MUI Data grid.
        private const string INTERNAL_ID_KEY = "internalId";

        private readonly IJsonSerializer _jsonSerializer;

        private readonly TypeDataManager _typeDataManager;

        public UpdateRowsController(
            IJsonSerializer jsonSerializer,
            TypeDataManager typeDataManager) 
        {
            _jsonSerializer = jsonSerializer;
            _typeDataManager = typeDataManager;
        }

        [HttpPatch]
        [Route("/type/update-rows")]
        public RES_UpdateConfigRows Process([FromBody] REQ_UpdateConfigRows req)
        {
            var filteredRows = FilterInternalIdOrDefault(req.Rows);
            if (default == filteredRows)
            {
                throw new ServerException(ServerErrorCode.ConfigData_UpdateRow, "failed to convert rows.");
            }

            if (false == _typeDataManager.UpdateRows(req.Schema, filteredRows))
            {
                throw new ServerException(ServerErrorCode.ConfigData_UpdateRow, "failed to update row");
            }

            return new RES_UpdateConfigRows();
        }

        /// <summary>
        /// Filter internal id key from original rows.
        /// </summary>
        private List<TypeSchemaRow> FilterInternalIdOrDefault(List<object> oriRows)
        {
            var dstRows = new List<TypeSchemaRow>();

            foreach (var row in oriRows)
            {
                var jsonRow = _jsonSerializer.Serialize(row);
                var rowDict = _jsonSerializer.Deserialize<Dictionary<string, object>>(jsonRow);

                // remove INTERNAL_ID_KEY.
                if (rowDict.ContainsKey(INTERNAL_ID_KEY))
                {
                    rowDict.Remove(INTERNAL_ID_KEY);
                }

                // check Id, Name
                if (false == rowDict.ContainsKey("Id"))
                {
                    return default;
                }

                if (false == rowDict.ContainsKey("Name"))
                {
                    return default;
                }

                if (string.IsNullOrEmpty(rowDict["Name"].ToString()))
                {
                    return default;
                }

                // schemaRow
                var schemaRow = new TypeSchemaRow
                {
                    Id = int.Parse(rowDict["Id"].ToString()),
                    Name = rowDict["Name"].ToString(),
                    Summary = string.Empty,
                };
                
                // add row
                dstRows.Add(schemaRow);

                if (rowDict.ContainsKey("Summary"))
                {
                    schemaRow.Summary = rowDict["Summary"]?.ToString();
                }
            }

            return dstRows;
        }
    }
}
