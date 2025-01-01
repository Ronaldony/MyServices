namespace DataDesigner.Core.TypeManager
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
        public bool AddOrUpdatgeRow(string rowName, string rowJson)
        {
            // Find type.
            var type = GetTypeFromName(rowName);
            if (null == type)
            {
                return false;
            }

            // Add row.
            var rowData = GetRowData(type, rowJson);
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
        public bool DeleteRow(string rowName, string rowJson)
        {
            // Find type.
            var type = GetTypeFromName(rowName);
            if (null == type)
            {
                return false;
            }

            // Add row.
            var rowData = GetRowData(type, rowJson);
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
        private object GetRowData(Type type, string rowJson)
        {
            var row = Activator.CreateInstance(type);

            var rowDict = _jsonSerializer.Deserialize<Dictionary<string, object>>(rowJson);

            foreach (var prop in type.GetProperties())
            {
                var rowValue = rowDict[prop.Name];

                prop.SetValue(row, rowValue);
            }

            return row;
        }

        /// <summary>
        /// Add or update row data.
        /// </summary>
        private bool AddOrUpdateRowData(Type type, object rowData)
        {
            // Get Id.
            var prop = type.GetProperty("Id");
            var updateId = (int)prop.GetValue(rowData, null);

            // Get type rows.
            var oriRows = GetRows(type.Name);
            if (oriRows == default)
            {
                // Fail.
                return false;
            }

            // Find row.
            foreach (var row in oriRows.Select((value, index) => (value, index)))
            {
                var rowValue = row.value;
                var rowId = (int)prop.GetValue(rowValue, null);

                // Compare row Id.
                if (rowId == updateId)
                {
                    // 동일 Id인 경우 Replace.
                    oriRows.RemoveAt(row.index);
                    oriRows.Insert(row.index, rowData);

                    return true;
                }
                else if (rowId > updateId)
                {
                    // Id가 큰 경우.
                    oriRows.Insert(row.index, rowData);
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
            var updateId = (int)prop.GetValue(rowData, null);

            // Get type rows.
            var oriRows = GetRows(type.Name);
            if (oriRows == default)
            {
                // Fail.
                return false;
            }

            // Find row.
            foreach (var row in oriRows.Select((value, index) => (value, index)))
            {
                var rowValue = row.value;
                var rowId = (int)prop.GetValue(rowValue, null);

                // Compare row Id.
                if (rowId == updateId)
                {
                    // 동일 Id인 경우 Delete.
                    oriRows.RemoveAt(row.index);
                    return true;
                }
            }

            return false;
        }
    }
}
