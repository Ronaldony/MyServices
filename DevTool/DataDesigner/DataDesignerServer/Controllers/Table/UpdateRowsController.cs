
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.ConfigData;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Table;
	using DataDesignerServer.Core.Services.Interfaces;

	/// <summary>
	/// UpdateRowsController.
	/// </summary>
	public class UpdateRowsController : Controller
    {
        // internal id key for MUI Data grid.
        private const string INTERNAL_ID_KEY = "internalId";

        private readonly IJsonSerializer _jsonSerializer;

        private readonly TableDataManager _tableDataManager;

        public UpdateRowsController(
            IJsonSerializer jsonSerializer,
			TableDataManager tableDataManager) 
        {
            _jsonSerializer = jsonSerializer;
            _tableDataManager = tableDataManager;
        }

        [HttpPatch]
        [Route("/table/update-rows")]
        public RES_UpdateTableRows Process([FromBody] REQ_UpdateTableRows req)
        {
            var filteredRows = FilterInternalId(req.Rows);

            if (false == _tableDataManager.UpdateRows(req.Schema, filteredRows))
            {
                throw new ServerException(ServerErrorCode.TableData_UpdateRow, "failed to update row");
            }

            return new RES_UpdateTableRows();
        }

        /// <summary>
        /// Filter internal id key from original rows.
        /// </summary>
        private List<object> FilterInternalId(List<object> oriRows)
        {
            var dstRows = new List<object>();

            foreach (var row in oriRows)
            {
                var jsonRow = _jsonSerializer.Serialize(row);
                var rowDict = _jsonSerializer.Deserialize<Dictionary<string, object>>(jsonRow);

                // remove INTERNAL_ID_KEY.
                if (rowDict.ContainsKey(INTERNAL_ID_KEY))
                {
                    rowDict.Remove(INTERNAL_ID_KEY);
                }

                dstRows.Add(_jsonSerializer.Serialize(rowDict));
            }

            return dstRows;
        }
    }
}
