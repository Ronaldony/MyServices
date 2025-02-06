using Newtonsoft.Json.Linq;

namespace DataDesignerServer.Core.DataManager.ConfigData
{
    /// <summary>
    /// ConfigDataManager.
    /// Desc: Manage config types.
    /// </summary>
    public sealed partial class ConfigDataManager
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
        /// Update members.
        /// </summary>
        private bool AddOrUpdatgeRow(string className, object row)
        {
            // Find type.
            var type = GetClassType(className);
            if (null == type)
            {
                return false;
            }

            // Add row.
            var rowData = GetRowData(type, row);
            if (false == AddOrUpdateRowData(type, rowData))
            {
                return false;
            }

            // Write update rows to the file.
            if (false == FlushFiles(type.Name))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update members.
        /// </summary>
        private bool DeleteRow(string className, object row)
        {
            // Find type.
            var type = GetClassType(className);
            if (null == type)
            {
                return false;
            }

            // Add row.
            var rowData = GetRowData(type, row);
            if (false == DeleteRowData(type, rowData))
            {
                return false;
            }

            // Write update rows to the file.
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

                if (default != propType)
                {
                    object value = default;

                    // enum type인 경우
                    propType = _typeDataManager.FindTypeOrDefault(prop.PropertyType.Name);

                    if (rowDict.ContainsKey(prop.Name))
                    {
                        value = Enum.Parse(propType, rowDict[prop.Name].ToString());
                    }
                    else
                    {
                        var defaultValue = propType.GetEnumValues().GetValue(0).ToString();
                        value = Enum.Parse(propType, defaultValue);
                    }

                    prop.SetValue(rowData, value);
                }
                else
                {
                    // primitive type인 경우
                    var value = Convert.ChangeType(rowDict[prop.Name], prop.PropertyType);
                    prop.SetValue(rowData, value);
                }

            }

            return rowData;
        }

        /// <summary>
        /// Add or update row data.
        /// </summary>
        private bool AddOrUpdateRowData(Type type, object rowData)
        {
            // Get Id.
            var prop = type.GetProperty("Id");
            var updateId = (int)prop.GetValue(rowData);

            // Get type rows.
            var oriRows = GetRowsOrDefault(type.Name);
            if (oriRows == default)
            {
                // Fail.
                return false;
            }

            // Find row.
            foreach (var oriRow in oriRows.Select((value, index) => (value, index)))
            {
                var oriRowId = (int)prop.GetValue(oriRow.value);

                // Compare row Id.
                if (oriRowId == updateId)
                {
                    // 동일 Id인 경우 Replace.
                    oriRows.RemoveAt(oriRow.index);
                    oriRows.Insert(oriRow.index, rowData);

                    return true;
                }
                else if (oriRowId > updateId)
                {
                    // Id가 큰 경우.
                    oriRows.Insert(oriRow.index, rowData);
                    return true;
                }
            }

            // 새로운 Id의 값이 제일 큰 경우.
            oriRows.Add(rowData);

            return true;
        }

        /// <summary>
        /// Delete row data.
        /// </summary>
        private bool DeleteRowData(Type type, object rowData)
        {
            // Get Id.
            var prop = type.GetProperty("Id");
            var updateId = (int)prop.GetValue(rowData);

            // Get type rows.
            var oriRows = GetRowsOrDefault(type.Name);
            if (oriRows == default)
            {
                // Fail.
                return false;
            }

            // Find row.
            foreach (var oriRow in oriRows.Select((value, index) => (value, index)))
            {
                var oriRowId = (int)prop.GetValue(oriRow.value);

                // Compare row Id.
                if (oriRowId == updateId)
                {
                    // 동일 Id인 경우 Delete.
                    oriRows.RemoveAt(oriRow.index);
                    return true;
                }
            }

            return false;
        }
    }
}
