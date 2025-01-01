
using System.Collections;
using System.Data;

namespace DataDesigner.Core.TypeManager
{
    using DataDesigner.Core.Data;
    using System.Reflection.Emit;

    /// <summary>
    /// ClassManager.
    /// Desc: Manage class types.
    /// </summary>
    public sealed partial class ClassManager
    {
        /// <summary>
        /// Create schema for class.
        /// </summary>
        public bool UpdateSchema(string schemaName, string schemaJson)
        {
            // Old schema.
            var oldSchema = GetSchema(schemaName);
            if (null == oldSchema)
            {
                return false;
            }

            var newSchema = _jsonSerializer.Deserialize<ClassSchema>(schemaJson);

            // Check column.
            if (IsColumnChanged(oldSchema.Columns, newSchema.Columns))
            {
                var newTB = CreateTypeBuilder(newSchema.Name, newSchema);

                // 컬럼이 변경된 경우, 데이터 파일도 변경이 필요.
                if (false == UpdateSchemaRows(newTB, newSchema, oldSchema.Columns, newSchema.Columns))
                {
                    return false;
                }

                // Update type buidler.
                AddOrUpdateTypeBuilder(newSchema.Name, newTB);

                FlushFiles(newSchema.Name);
            }

            // Set schema.
            if (default == SetSchema(schemaName, newSchema))
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
        /// Check column of schema is changed .
        /// </summary>
        private bool IsColumnChanged(List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            // Cond 1. 컬럼 명 및 개수.
            var interCols = oldCols.IntersectBy(newCols.Select(d => d.Name), d => d.Name);

            // 교집합 개수.
            if (oldCols.Count() != interCols.Count())
            {
                return false;
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
        private bool UpdateSchemaRows(TypeBuilder tb, ClassSchema newSchema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            // Delete rows.
            var isDelete = DeleteRowColumns(tb, newSchema, oldCols, newCols);
            if (false == isDelete)
            {
                return false;
            }

            // Add rows.
            var isAdded = AddRowColumns(newSchema, oldCols, newCols);
            if (false == isAdded)
            {
                return false;
            }

            // Update rows.
            var isUpdated = UpdateRowColumns(newSchema, oldCols, newCols);
            if (false == isUpdated)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete rows for deleted column.
        /// </summary>
        private bool DeleteRowColumns(TypeBuilder tb, ClassSchema newSchema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            var deleteColNames = new List<string>();

            foreach (var oldCol in oldCols)
            {
                var findCol = newCols.FirstOrDefault(d => d.Idx.Equals(oldCol.Idx));

                // 스키마 컬럼 Idx가 없는 경우, 해당 컬럼 삭제.
                if (null == findCol)
                {
                    deleteColNames.Add(oldCol.Name);
                }
            }

            // Get rows.
            var oriRows = GetRows(newSchema.Name);
            if (default ==  oriRows)
            {
                return false;
            }

            // Create List<Type>.
            var schemaListType = typeof(List<>).MakeGenericType(tb.CreateType());
            var schemaTypes = (IList)Activator.CreateInstance(schemaListType);

            var properties = tb.CreateType().GetProperties();

            foreach (var oriRow in oriRows)
            {
                var type = tb.CreateType();

                // Set property.
                foreach (var prop in properties)
                {
                    var findName = deleteColNames.FirstOrDefault(d => d.Equals(prop.Name));
                    if (string.IsNullOrEmpty(findName))
                    {
                        continue;
                    }

                    var value = prop.GetValue(oriRow, null);
                    var setProp = type.GetProperty(prop.Name);

                    // 새로운 Schema에 Property가 없음
                    if (null == setProp)
                    {
                        return false;
                    }

                }

                schemaTypes.Add(type);
            }

            // Update ori rows.

            return true;
        }

        /// <summary>
        /// Add rows for added column.
        /// </summary>
        private bool AddRowColumns(ClassSchema schema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {

            return true;
        }

        /// <summary>
        /// Update rows for updated column.
        /// </summary>
        private bool UpdateRowColumns(ClassSchema schema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {

            return true;
        }

        /// <summary>
        /// Check whether type can convert oriType to dstType.
        /// </summary>
        private bool CanTypeChanged(string oriTypeName, string dstTypeName)
        {
            try
            {
                var oriType = GetTypeFromName(oriTypeName);
                var dstType = GetTypeFromName(dstTypeName);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError($"Cannot change type from {oriTypeName} to {dstTypeName}");
                return false;
            }

            return true;
        }
    }
}
