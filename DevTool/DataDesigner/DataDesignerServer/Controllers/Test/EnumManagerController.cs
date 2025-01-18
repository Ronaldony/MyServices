
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Test
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.Http.Enum;
    using DataDesignerServer.Core.Services.Interfaces;
    using DataDesignerServer.Core.TypeManager.Enum;

    public class EnumManagerController : Controller
    {
        private readonly ILogger<EnumManagerController> _logger;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly EnumManager _enumManaer;

        public EnumManagerController(
            IServiceProvider serviceProvider)
        { 
            _logger = serviceProvider.GetService<ILogger<EnumManagerController>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _enumManaer = serviceProvider.GetService<EnumManager>();
        }

        [HttpPost]
        [Route("test/enum-manager/create-schema")]
        public string CreateSchema([FromBody]EnumSchema schema)
        {
            if (false == _enumManaer.CreateSchema(schema.Name, schema))
            {
                return "fail.";
            }

            return $"create schema. name: {schema.Name}";
        }

        [HttpGet]
        [Route("test/enum-manager/rows")]
        public string FindSchema(string enumName)
        {
            var rows = _enumManaer.GetRowsOrDefault(enumName);

            if (default == rows)
            {
                return $"not exist rows. name: {enumName}";
            }

            return _jsonSerializer.Serialize(rows);
        }

        [HttpGet]
        [Route("test/enum-manager/all-enums")]
        public string AllEnums()
        {
            var schemaNames = _enumManaer.GetSchemaNames();

            if (false == schemaNames.Any())
            {
                return "empty.";
            }

            return string.Join(", ", schemaNames);
        }

        [HttpPost]
        [Route("test/enum-manager/update-rows")]
        public string UpdateRows([FromBody] REQ_EnumData req)
        {
            if (false == _enumManaer.UpdateRows(req.EnumName, req.Rows))
            {
                return "fail.";
            }

            return "update rows.";
        }
    }
}
