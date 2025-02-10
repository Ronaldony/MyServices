using Newtonsoft.Json.Linq;

namespace DataDesignerServer.Core.DataManager.TableData
{
	/// <summary>
	/// TableDataManager.
	/// Desc: Manage table types.
	/// </summary>
	public sealed partial class TableDataManager
    {
        public bool UpdateRows(string className, List<object> rows)
        {
            // Find type.
            var type = GetClassType(className);
            if (null == type)
            {
                return false;
            }

            // Get type rows.
            var oriRows = GetRowsOrDefault(type.Name);
            if (oriRows == default)
            {
                // Fail.
                return false;
            }

            oriRows.Clear();

            foreach (var row in rows)
            {
                // Add row.
                var rowData = GetRowData(type, row);
                oriRows.Add(rowData);
            }

            if (false == FlushFiles(type.Name))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get row data from row json.
        /// </summary>
        private object GetRowData(Type type, object row)
        {
            var rowDict = _jsonSerializer.Deserialize<Dictionary<string, object>>(row.ToString());
            var rowData = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                var propType = _typeDataManager.FindTypeOrDefault(prop.PropertyType.Name);
                object value = default;

                if (default != propType)
                {
                    // enum type인 경우
                    propType = _typeDataManager.FindTypeOrDefault(prop.PropertyType.Name);

                    if (rowDict.ContainsKey(prop.Name))
                    {
                        // parse data.
                        value = Enum.Parse(propType, rowDict[prop.Name].ToString());
                    }
                    else
                    {
                        // Default value.
                        var defaultValue = propType.GetEnumValues().GetValue(0).ToString();
                        value = Enum.Parse(propType, defaultValue);
                    }

                    prop.SetValue(rowData, value);
                }
                else
                {
                    // primitive type인 경우
                    if (rowDict.ContainsKey(prop.Name))
                    {
                        value = Convert.ChangeType(rowDict[prop.Name], prop.PropertyType);
                    }
                    else
                    {
                        value = default;
                    }

                    prop.SetValue(rowData, value);
                }

            }

            return rowData;
        }
    }
}
