
using System.Collections;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.DataManager.ConfigData
{
	using DataDesignerServer.Core.Data;

	/// <summary>
	/// ConfigDataManager.
	/// Desc: Manage config types.
	/// </summary>
    public sealed partial class ConfigDataManager
    {
        /// <summary>
        /// Create schema.
        /// </summary>
        public bool CreateSchema(string schemaName, ClassSchema schema)
        {
            // Schema.
            var oldSchema = FindSchemaOrDefault(schemaName);
            if (null != oldSchema)
            {
                // Schema가 이미 있는 경우.
                return false;
            }

			// Schema.
			if (default == SetSchema(schemaName, schema))
            {
                return false;
            }

            // TypeBuilder.
            var tb = CreateTypeBuilder(schemaName, schema);

            // Set row, builder.
            SetRows(schemaName, new List<object>());
            SetTypeBuilder(schemaName, tb);

            // Flush.
            if (false == FlushFiles(schemaName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create schema for class.
        /// </summary>
        public bool UpdateSchema(string schemaName, ClassSchema schema)
        {
            // Old schema.
            var oldSchema = FindSchemaOrDefault(schemaName);
            if (null == oldSchema)
            {
                return false;
            }

            // Check column.
            if (IsColumnChanged(oldSchema.Columns, schema.Columns))
            {
                var oldTB = GetTypeBuilder(oldSchema.Name);
                var newTB = CreateTypeBuilder(schema.Name, schema);

                // 컬럼이 변경된 경우.
                if (false == UpdateSchemaRows(oldTB, newTB, schema, oldSchema.Columns, schema.Columns))
                {
                    return false;
                }

                // Update type buidler.
                SetTypeBuilder(schema.Name, newTB);
            }

            // Reset schema Idx.
            foreach (var selectData in schema.Columns.Select((value, index) => (value, index)))
            {
                var column = selectData.value;
                column.Idx = selectData.index + 1;
            }

            // Set schema.
            if (default == SetSchema(schemaName, schema))
            {
                return false;
            }
            
            // Refresh schema file.
            if (false == FlushFiles(schemaName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete schema
        /// </summary>
		public bool DeleteSchema(string schemaName)
		{
			// Old schema.
			var oldSchema = FindSchemaOrDefault(schemaName);
			if (null == oldSchema)
			{
				return false;
			}

            // remove rows.
            if (false == RemoveRows(schemaName))
            {
                return false;
            }

			// remove schema.
			if (false == RemoveSchema(schemaName))
			{
				return false;
			}

            // Refresh schema file.
            RemoveFiles(schemaName);

			return true;
		}

		/// <summary>
		/// Check column of schema is changed .
		/// </summary>
		private bool IsColumnChanged(List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            // Cond 1. 컬럼 명 및 개수.
            var interCols = oldCols.IntersectBy(newCols.Select(d => d.Name), d => d.Name);

            // Column count.
            if (oldCols.Count() != newCols.Count())
            {
                return true;
            }

            // 교집합 개수.
            if (oldCols.Count() != interCols.Count())
            {
                return true;
            }

            // Cond 2. 컬럼 타입.
            foreach (var oldCol in oldCols)
            {
                var newCol = newCols.FirstOrDefault(d => d.Name.Equals(oldCol.Name));

                // Type이 다른 경우.
                if (false == newCol.Type.Equals(oldCol.Type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updat rows by columns.
        /// </summary>
        private bool UpdateSchemaRows(TypeBuilder oldTB, TypeBuilder newTB, ClassSchema newSchema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            var addCols = GetAddCols(newCols);
            var updateCols = GetUpdateCols(oldCols, newCols);

            // Get rows.
            var oriRows = GetRowsOrDefault(newSchema.Name);
            if (default == oriRows)
            {
                return false;
            }

            // Create List for new type.
            var classListType = typeof(List<>).MakeGenericType(newTB.CreateType());
            var classList = (IList)Activator.CreateInstance(classListType);

            var newType = newTB.CreateType();
            var oldProps = oldTB.CreateType().GetProperties();

            // Search original row.
            foreach (var oriRow in oriRows)
            {
                var typeInst = Activator.CreateInstance(newType);

                // Search new props.
                foreach (var prop in newType.GetProperties())
                {
                    object value = default;

                    if (addCols.Exists(d => d.Equals(prop.Name)))
                    {
                        // Add column.
                        var findType = _typeDataManager.FindTypeOrDefault(prop.PropertyType.Name);
                        if (default != findType)
                        {
                            // Default value
                            value = findType.GetEnumValues().GetValue(0);
                        }
                    }
                    else if (updateCols.ContainsKey(prop.Name))
                    {
                        // Update column.
                        var oldProp = oldProps.FirstOrDefault(d => d.Name.Equals(updateCols[prop.Name]));
                        value = oldProp.GetValue(oriRow);
                        
                        // Check type.
                        if (false == CanTypeChanged(value, prop.PropertyType))
                        {
                            return false;
                        }

                        // convert value
                        value = Convert.ChangeType(value, prop.PropertyType);
                    }
                    else
                    {
                        // Old colmn.
                        var oldProp = oldProps.FirstOrDefault(d => d.Name.Equals(prop.Name));
                        value = oldProp.GetValue(oriRow);
                    }

                    // Old column.
                    prop.SetValue(typeInst, value);
                }

                classList.Add(typeInst);
            }

            // Class datas.
            var classDatas = classList.OfType<object>().ToList();
            SetRows(newSchema.Name, classDatas);

            return true;
        }

        /// <summary>
        /// Add rows for added column.
        /// </summary>
        private List<string> GetAddCols(List<ClassSchemaColumn> newCols)
        {
            return newCols.Where(d => d.Idx == 0).Select(d => d.Name).ToList();
        }

        /// <summary>
        /// Update rows for updated column.
        /// </summary>
        /// <returns>
        /// key: new column name.
        /// value: old column name.
        /// </returns>
        private Dictionary<string, string> GetUpdateCols(List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            var colDict = new Dictionary<string, string>();

            foreach (var newCol in newCols)
            {
                var oldCol = oldCols.FirstOrDefault(d => d.Idx.Equals(newCol.Idx));
                if (default != oldCol)
                {
                    // cond 1. name
                    if (false == oldCol.Name.Equals(newCol.Name))
                    {
                        colDict.Add(newCol.Name, oldCol.Name);
                        continue;
                    }

                    // cond 2. type
                    if (false == oldCol.Type.Equals(newCol.Type))
                    {
                        colDict.Add(newCol.Name, oldCol.Name);
                        continue;
                    }
                }
            }

            return colDict;
        }

        /// <summary>
        /// Check whether type can convert oriType to dstType.
        /// </summary>
        private bool CanTypeChanged(object value, Type type)
        {
            try
            {
                var typeValue = Convert.ChangeType(value, type);
                if (null == typeValue)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }
    }
}
