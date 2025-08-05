
using System.Reflection.Emit;

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
            var tb = FindTableBuilder(className);
            if (null == tb)
            {
                return false;
            }

            // Create type.
            var type = tb.CreateType();

            // Get new data rows.
            var newRows = GetNewRows(type, rows);
            if (null == newRows)
            {
                // Table 빌더 Refresh.
                var classGroup = GetClassGroup(className);
                var newTB = CreateTypeBuilder(classGroup.Schema.Name, classGroup.Schema);

                AddOrUpdateClassGroup(classGroup.Schema.Name, tb: newTB);

                // Get new rows.
                newRows = GetNewRows(classGroup.Builder.CreateType(), rows);
            }

            AddOrUpdateClassGroup(className, rows: newRows);

            if (false == FlushFiles(type.Name))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get new data rows.
        /// </summary>
        private List<object> GetNewRows(Type type, List<object> rows)
        {
            var newRows = new List<object>();
            foreach (var row in rows)
            {
                // Add row.
                var rowData = GetRowData(type, row);

                if (null == rowData)
                {
                    // Table 빌더 Refresh.
                    return null;
                }

                newRows.Add(rowData);
            }

            return newRows;
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

                    if (false == prop.GetValue(rowData).Equals(value))
                    {
                        // Type이 변경되었지만, Table에는 반영되지 않은 경우 발생.
                        // 이 경우, Table 빌더를 Refresh 해줘야 함.
                        return null;
                    }
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
