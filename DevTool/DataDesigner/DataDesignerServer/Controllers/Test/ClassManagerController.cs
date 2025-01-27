
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Test
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.Http.Test;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// Test_DataMapperController.
    /// </summary>
    public class ClassManagerController : Controller
    {
        private readonly ILogger<ClassManagerController> _logger;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly TableDataManager _classManaer;

        public ClassManagerController(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassManagerController>>();
            _jsonSerializer = serviceProvider.GetService<IJsonSerializer>();
             
            _classManaer = serviceProvider.GetRequiredService<TableDataManager>();
        }

        [HttpGet]
        [Route("test/class-manager/test-all")]
        public string TestClassManager()
        {
            // Create
            Create_Schema("TB_Test", "Test", "Test schema");

            return "Result OK.";
        }

        [HttpGet]
        [Route("test/class-manager/find-schema-names")]
        public string FindSchemaNames()
        {
            var schemaNames = _classManaer.GetSchemaNames();
            if (schemaNames.Any() == false)
            {
                return "not exist schema";
            }
            
            return string.Join(", ", schemaNames);
        }

        [HttpGet]
        [Route("test/class-manager/find-schema")]
        public string FindSchema(string schemaName)
        {
            var findSchema = _classManaer.FindSchemaOrDefault(schemaName);

            if (default == findSchema)
            {
                return $"cannot find schema. find: {schemaName}";
            }

            return _jsonSerializer.Serialize(findSchema);
        }

        [HttpGet]
        [Route("test/class-manager/create-schema")]
        public string Create_Schema(string schemaName, string category, string desc)
        {
            var schema = new ClassSchema
            {
                Name = schemaName,
                Category = category,
                Columns = new List<ClassSchemaColumn>(),
                Desciption = desc
            };

            // Create.
            if (false == _classManaer.CreateSchema(schema.Name, schema))
            {
                return "fail.";
            }

            return "Result OK.";
        }

        [HttpPost]
        [Route("test/class-manager/update-column")]
        public string ClassManager_UpdateColumn([FromBody] ClassSchema schema)
        {
            // Update schema.
            if (false == _classManaer.UpdateSchema(schema.Name, schema))
            {
                return "fail.";
            }

            return "update column.";
        }

        [HttpPost]
        [Route("test/class-manager/add-update-row")]
        public string ClassManager_AddOrUpdateRow([FromBody] REQ_ClassData req)
        {
            if (false == _classManaer.AddOrUpdatgeRow(req.ClassName, req.Row))
            {
                return "fail.";
            }

            return "add row ok.";
        }

        [HttpPost]
        [Route("test/class-manager/delete-row")]
        public string ClassManager_DeleteRow([FromBody] REQ_ClassData req)
        {
            if (false == _classManaer.DeleteRow(req.ClassName, req.Row))
            {
                return "fail.";
            }

            return "delete row ok.";
        }
    }
}
