
using Microsoft.AspNetCore.Mvc;

namespace DataDesignerServer.Controllers.Common
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.DataManager.ClassData;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Http.Common;
    using DataDesignerServer.Core.Services.Interfaces;
    using DataDesignerServer.Core.Types;

    /// <summary>
    /// FetchTypeOrTableKeysController.
    /// </summary>
    public class FetchTypeOrTableKeysController : Controller
	{
		private readonly IJsonSerializer _jsonSerializer;

		private readonly ClassDataManager _classDataManager;
		private readonly TypeDataManager _typeDataManager;

		public FetchTypeOrTableKeysController(
			IJsonSerializer jsonSerializer,
            ClassDataManager classDataManager,
			TypeDataManager typeDataManager) 
		{
			_jsonSerializer = jsonSerializer;

			_classDataManager = classDataManager;
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
			var typeNames = _typeDataManager.GetAllType();

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
			var tableNames = _classDataManager.GetClassTypes(Type_ClassData.Table).Select(d => d.Name);

			foreach (var tableName in  tableNames)
			{
				var findRows = _classDataManager.FindRowsOrDefault(tableName);

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
