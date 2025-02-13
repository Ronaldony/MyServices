
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Test
{
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.DataManager.TypeData;
	using DataDesignerServer.Core.Generator;
	using DataDesignerServer.Core.Types;

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
			var classSchemaNames = _classManager.GetAllType().Select(d => d.Name);

			foreach (var name in classSchemaNames)
			{
				// Find schema.
				var schema = _classManager.FindSchemaOrDefault(name);
				if (null == schema)
				{
					_logger.LogDebug($"not exist schema. name: {name}");
					continue;
				}

				_codeGenerator.AddClass(Type_GenerateCode.Table, "GameCore", schema.Name, schema.Columns);
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

				_codeGenerator.AddEnum(Type_GenerateCode.Type, "GameCore", schema.Name, rows);
			}

			var createFolder = Path.Combine(Directory.GetCurrentDirectory(), "Code");
			_codeGenerator.CreateFiles(createFolder);

			return "GenerateEnum OK.";
		}
	}
}
