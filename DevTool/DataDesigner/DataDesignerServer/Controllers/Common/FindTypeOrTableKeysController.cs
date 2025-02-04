
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
		[Route("/common/find-type-table-keys")]
		public RES_FindTypeOrTableKeys Process(string schema)
		{
			if (string.IsNullOrEmpty(schema))
			{
                throw new ServerException(ServerErrorCode.Common_NotExistSchema, "not exist schema");
            }

            // find from type.
            if (default != _typeDataManager.FindSchemaOrDefault(schema))
			{
				var rows = _typeDataManager.GetRowsOrDefault(schema);

				return new RES_FindTypeOrTableKeys
				{
					Keys = rows.Select(d => d.Name).ToList()
				};

			}

			// find from table.
			if (default != _tableDataManager.FindSchemaOrDefault(schema))
			{
				var rows = _tableDataManager.FindRowsOrDefault(schema);

				return new RES_FindTypeOrTableKeys
				{
					Keys = FilterIdFromTable(rows),
				};
			}

			throw new ServerException(ServerErrorCode.Common_NotExistSchema, "not exist schema");
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
