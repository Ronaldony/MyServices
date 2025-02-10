
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.Http.Table;

	/// <summary>
	/// FindSchemaController.
	/// </summary>
	public class FindSchemaController : Controller
    {
        private readonly TableDataManager _tableDataManager;

        public FindSchemaController(
			TableDataManager tableDataManager)
        {
            _tableDataManager = tableDataManager;
        }

        [HttpGet]
        [Route("/table/find-schema")]
        public RES_FindTableSchema Process(string schema)
        {
            var findSchema = _tableDataManager.FindSchemaOrDefault(schema);
            if (null == findSchema)
            {
                throw new ServerException(ServerErrorCode.TableData_NotExistSchema, "not exist schema");
            }

            return new RES_FindTableSchema
			{
                Schema = findSchema
            };
        }
    }
}
