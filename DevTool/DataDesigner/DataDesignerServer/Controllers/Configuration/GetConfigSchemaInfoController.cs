
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Configuration
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.Http.Configuration;

    public class GetConfigSchemaInfoController : Controller
    {
        private readonly ILogger<GetConfigSchemaInfoController> _logger;

        public GetConfigSchemaInfoController()
        {
        }

        [HttpGet]
        [Route("/config/get-schema-info")]
        public RES_GetConfigSchemaInfo Process()
        {
            var schema = new ClassSchema();
            schema.Name = "TestSchema";

            return new RES_GetConfigSchemaInfo
            {
                Schema = schema
            };
        }
    }
}
