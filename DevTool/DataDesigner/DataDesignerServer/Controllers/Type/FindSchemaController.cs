
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Configuration;

    /// <summary>
    /// FindSchemaController.
    /// </summary>
    public class FindSchemaController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public FindSchemaController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpGet]
        [Route("/type/find-schema")]
        public RES_FindTypeSchema Process(string schema)
        {
            var findSchema = _typeDataManager.FindSchemaOrDefault(schema);
            if (null == findSchema)
            {
                throw new ServerException(ServerErrorCode.TypeData_NotExistSchema, "not exist schema");
            }

            return new RES_FindTypeSchema
            {
                Schema = findSchema
            };
        }
    }
}
