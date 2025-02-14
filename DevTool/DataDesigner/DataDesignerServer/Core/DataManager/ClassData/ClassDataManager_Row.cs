
namespace DataDesignerServer.Core.DataManager.ClassData
{
    /// <summary>
    /// TableDataManager.
    /// Desc: Manage table types.
    /// </summary>
    public sealed partial class ClassDataManager
    {
        public bool UpdateRows(string className, List<object> rows)
        {
            // Find type.
            var tb = FindTypeBuilder(className);
            if (null == tb)
            {
                return false;
            }

            // Create type.
            var type = tb.CreateType();

            var newRows = new List<object>();
            foreach (var row in rows)
            {
                // Add row.
                var rowData = GetRowData(type, row);
                newRows.Add(rowData);
            }

            AddOrUpdateClassGroup(className, rows: newRows);

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
