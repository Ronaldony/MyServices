
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Configuration;

    /// <summary>
    /// UpdateSchemaController.
    /// </summary>
    public class UpdateSchemaController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public UpdateSchemaController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpPost]
        [Route("/config/update-schema")]
        public RES_UpdateConfigSchema Process([FromBody] REQ_UpdateConfigSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default == _classDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.ConfigData_NotExistSchema, "not exist schema");
            }

			// Check columns
			foreach (var column in req.Schema.Columns)
			{
				if (string.IsNullOrEmpty(column.Name))
				{
					throw new ServerException(ServerErrorCode.ConfigData_NoColumnName, $"no column name.");
				}

				if (false == string.IsNullOrEmpty(column.RefTable))
				{
					column.Type = "int";
				}
			}

			// update schema    
			if (false == _classDataManager.UpdateSchema(req.Schema.Name, req.Schema))
            {
				throw new ServerException(ServerErrorCode.ConfigData_UpdateSchema, "fail to update schema");
			}

            return new RES_UpdateConfigSchema();
        }
    }
}
