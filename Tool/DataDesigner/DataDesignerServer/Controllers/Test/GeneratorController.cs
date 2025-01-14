using DataDesignerServer.Core.Data;
using DataDesignerServer.Core.Generator;
using DataDesignerServer.Core.TypeManager.Class;
using DataDesignerServer.Core.TypeManager.Enum;
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Test
{
    /// <summary>
    /// GeneratorController.
    /// </summary>
    public class GeneratorController : Controller
    {
        private readonly ILogger<GeneratorController> _logger;

        private readonly CodeGenerator _codeGenerator;
        private readonly ClassManager _classManager;
        private readonly EnumManager _enumManager;

        public GeneratorController(IServiceProvider serviceProvider) 
        {
            _logger = serviceProvider.GetRequiredService<ILogger<GeneratorController>>();

            _codeGenerator = serviceProvider.GetRequiredService<CodeGenerator>();
            _classManager = serviceProvider.GetRequiredService<ClassManager>();
            _enumManager = serviceProvider.GetRequiredService<EnumManager>();
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
            var enumSchemaNames = _enumManager.GetSchemaNames();

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

                var members = new List<EnumMember>();
                
                // Rows.
                foreach (var row in rows)
                {
                    members.Add(new EnumMember
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
