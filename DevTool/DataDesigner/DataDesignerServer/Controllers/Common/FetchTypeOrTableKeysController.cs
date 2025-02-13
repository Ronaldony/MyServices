
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
	using DataDesignerServer.Core.Common;
	using DataDesignerServer.Core.DataManager.TableData;
	using DataDesignerServer.Core.DataManager.TypeData;
	using DataDesignerServer.Core.Http.Common;
	using DataDesignerServer.Core.Services.Interfaces;

	/// <summary>
	/// FetchTypeOrTableKeysController.
	/// </summary>
	public class FetchTypeOrTableKeysController : Controller
	{
		private readonly IJsonSerializer _jsonSerializer;

		private readonly TableDataManager _tableDataManager;
		private readonly TypeDataManager _typeDataManager;

		public FetchTypeOrTableKeysController(
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
			var typeNames = _typeDataManager.GetTypeSchemasWithPrimitive();

			foreach (var typeName in typeNames)
			{
				var rows = _typeDataManager.GetRowsOrDefault(typeName);

				if (default == rows)
				{
					// primitive types.
					continue;
				}

				res.SchemaKeysDict.Add(typeName, rows.Select(d => d.Name).ToList());
			}

			// find from table.
			var tableNames = _tableDataManager.GetAllType().Select(d => d.Name);

			foreach (var tableName in  tableNames)
			{
				var findRows = _tableDataManager.FindRowsOrDefault(tableName);

				if (default == findRows)
				{
					throw new ServerException(ServerErrorCode.Common_NotExistSchema, $"not exist schema. name: {tableName}");
				}

				// filter rows.
				var rows = FilterIdFromTable(findRows);

				res.SchemaKeysDict.Add(tableName, rows);
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
				var rowJson = _jsonSerializer.Serialize(row);
				var dict = _jsonSerializer.Deserialize<Dictionary<string, object>>(rowJson);

				tableIds.Add(dict["Id"].ToString());
			}

			return tableIds;
		}
	}
}
