
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ConfigData;
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

        private readonly ConfigDataManager _configDataManager;

        public UpdateRowsController(
            IJsonSerializer jsonSerializer,
            ConfigDataManager configDataManager) 
        {
            _jsonSerializer = jsonSerializer;
            _configDataManager = configDataManager;
        }

        [HttpPatch]
        [Route("/config/update-rows")]
        public RES_UpdateConfigRows Process([FromBody] REQ_UpdateConfigRows req)
        {
            var filteredRows = FilterInternalId(req.Rows);

            if (false == _configDataManager.UpdateRows(req.Schema, filteredRows))
            {
                throw new ServerException(ServerErrorCode.ConfigData_UpdateRow, "failed to update row");
            }

            return new RES_UpdateConfigRows();
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
