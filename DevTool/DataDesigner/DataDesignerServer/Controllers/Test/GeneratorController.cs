
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Test
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.TableData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Generator;

    /// <summary>
    /// GeneratorController.
    /// </summary>
    public class GeneratorController : Controller
    {
        private readonly ILogger<GeneratorController> _logger;

        private readonly CodeGenerator _codeGenerator;
        private readonly TableDataManager _classManager;
        private readonly TypeDataManager _enumManager;

        public GeneratorController(IServiceProvider serviceProvider) 
        {
            _logger = serviceProvider.GetRequiredService<ILogger<GeneratorController>>();

            _codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            _classManager = serviceProvider.GetRequiredService<TableDataManager>();
            _enumManager = serviceProvider.GetRequiredService<TypeDataManager>();
        }

        [HttpGet]
        [Route("generate-code")]
        public string GenerateCode()
        {
            //////////////////////////////////////////////
            /// Generate class.
            var classSchemaNames = _classManager.GetSchemaNames();

            foreach (var name in classSchemaNames)
            {
                // Find schema.
                var schema = _classManager.FindSchemaOrDefault(name);
                if (null == schema)
                {
                    _logger.LogDebug($"not exist schema. name: {name}");
                    continue;
                }

                var members = new List<ClassMember>();

                // Columns.
                foreach (var col in schema.Columns)
                {
                    members.Add(new ClassMember
                    {
                        Name = col.Name,
                        Type = col.Type,
                        IsIndexed = col.IsIndexed,
                        Summary = col.Summary,
                    });
                }

                _codeGenerator.AddClass("GameCore", schema.Name, members);
            }

            //////////////////////////////////////////////
            /// Generate enum.
            var enumSchemaNames = _enumManager.GetTypeSchemasWithPrimitive();

            foreach (var name in enumSchemaNames)
            {
                var schema = _enumManager.FindSchemaOrDefault(name);
                if (null == schema)
                {
                    _logger.LogDebug($"cannot find schema. name :{name}");
                    continue;
                }

                var rows = _enumManager.GetRowsOrDefault(name);
                if (default == rows)
                {
                    _logger.LogDebug($"cannot find rows. name :{name}");
                    continue;
                }

                var members = new List<TypeMember>();
                
                // Rows.
                foreach (var row in rows)
                {
                    members.Add(new TypeMember
                    {
                        Name = row.Name,
                        Value = row.Id,
                        Summary  = row.Summary
                    });
                }

                _codeGenerator.AddEnum("GameCore", schema.Name, members);
            }

            var createFolder = Path.Combine(Directory.GetCurrentDirectory(), "Code");
            _codeGenerator.CreateFiles(createFolder);

            return "GenerateEnum OK.";
        }
    }
}
