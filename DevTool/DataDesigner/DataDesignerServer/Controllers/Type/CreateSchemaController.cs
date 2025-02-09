using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Type
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Type;

    /// <summary>
    /// CreateSchemaController.
    /// </summary>
    public class CreateSchemaController : Controller
    {
        private readonly TypeDataManager _typeDataManager;

        public CreateSchemaController(
            TypeDataManager typeDataManager)
        {
            _typeDataManager = typeDataManager;
        }

        [HttpPost]
        [Route("/type/create-schema")]
        public RES_CreateTypeSchema Process([FromBody] REQ_CreateTypeSchema req)
        {
            // Name check.
            if (string.IsNullOrEmpty(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TypeData_NoSchemaName, "no schema name.");
            }

            // check existance for schema
            if (default != _typeDataManager.FindSchemaOrDefault(req.Schema.Name))
            {
                throw new ServerException(ServerErrorCode.TypeData_AlreadyExistSchema, "already exist schema");
            }

            // Create.
            if (false == _typeDataManager.CreateSchema(req.Schema.Name, req.Schema))
            {
                throw new ServerException(ServerErrorCode.TypeData_CreateSchema, "not exist configuration");
            }

            return new RES_CreateTypeSchema();
        }
    }
}
