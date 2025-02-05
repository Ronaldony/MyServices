
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.DataManager.TypeData;
	using DataDesignerServer.Core.Http.Common;
	using DataDesignerServer.Core.Services.Interfaces;

	/// <summary>
	/// FindTypeOrTableKeysController.
	/// </summary>
	public class FindTypeOrTableKeysController : Controller
	{
		private readonly IJsonSerializer _jsonSerializer;

		private readonly TableDataManager _tableDataManager;
		private readonly TypeDataManager _typeDataManager;

		public FindTypeOrTableKeysController(
			IJsonSerializer jsonSerializer,
			TableDataManager tableDataManager,
			TypeDataManager typeDataManager) 
		{
			_jsonSerializer = jsonSerializer;

			_tableDataManager = tableDataManager;
			_typeDataManager = typeDataManager;
		}

		[HttpGet]
		[Route("/common/fetch-type-table-keys")]
		public RES_FetchTypeAndTableKeys Process()
		{
			var res = new RES_FetchTypeAndTableKeys
			{
				SchemaKeysDict = new Dictionary<string, List<string>>()
			};

			// find from type.
			var typeSchemas = _typeDataManager.GetSchemaNames();

			foreach (var schema in typeSchemas)
			{
				var rows = _typeDataManager.GetRowsOrDefault(schema);

				if (default == rows)
				{
					// primitive types.
					continue;
				}

				res.SchemaKeysDict.Add(schema, rows.Select(d => d.Name).ToList());
			}

			// find from table.
			var tableSchemas = _tableDataManager.GetSchemaNames();

			foreach (var schema in  tableSchemas)
			{
				var findRows = _tableDataManager.FindRowsOrDefault(schema);

				if (default == findRows)
				{
					throw new ServerException(ServerErrorCode.Common_NotExistSchema, $"not exist schema. name: {schema}");
				}

				// filter rows.
				var rows = FilterIdFromTable(findRows);

				res.SchemaKeysDict.Add(schema, rows);
			}

			return res;
		}

		/// <summary>
		/// Fileter id values from table rows.
		/// </summary>
		private List<string> FilterIdFromTable(IEnumerable<object> rows)
		{
			var tableIds = new List<string>();

			foreach (var row in rows)
			{
				var dict = _jsonSerializer.Deserialize<Dictionary<string, string>>(row.ToString());

				tableIds.Add(dict["Id"]);
			}

			return tableIds;
		}
	}
}
