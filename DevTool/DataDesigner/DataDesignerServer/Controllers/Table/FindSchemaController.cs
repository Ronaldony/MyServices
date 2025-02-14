
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Table
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.Http.Table;

    /// <summary>
    /// FindSchemaController.
    /// </summary>
	public class FindSchemaController : Controller
    {
        private readonly ClassDataManager _classDataManager;

        public FindSchemaController(
            ClassDataManager classDataManager)
        {
            _classDataManager = classDataManager;
        }

        [HttpGet]
        [Route("/table/find-schema")]
        public RES_FindTableSchema Process(string schema)
        {
            var findSchema = _classDataManager.FindSchemaOrDefault(schema);
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
