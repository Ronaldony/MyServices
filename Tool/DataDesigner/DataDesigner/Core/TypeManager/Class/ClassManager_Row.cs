namespace DataDesignerServer.Core.TypeManager.Class
{
    /// <summary>
    /// ClassManager.
    /// Desc: Manage class types.
    /// </summary>
    public sealed partial class ClassManager
    {
        /// <summary>
        /// Update class members.
        /// </summary>
        public bool AddOrUpdatgeRow(string className, object row)
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
        /// Update class members.
        /// </summary>
        public bool DeleteRow(string className, object row)
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
                var propValue = Convert.ChangeType(rowDict[prop.Name], prop.PropertyType);

                prop.SetValue(rowData, propValue);
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
