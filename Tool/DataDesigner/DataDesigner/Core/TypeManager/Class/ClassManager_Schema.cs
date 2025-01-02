
using System.Collections;
using System.Data;

namespace DataDesigner.Core.TypeManager
{
    using DataDesigner.Core.Data;
    using Microsoft.OpenApi.Extensions;
    using System.Globalization;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Xml.Linq;

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
                var oldTB = GetTypeBuilder(oldSchema.Name);
                var newTB = CreateTypeBuilder(newSchema.Name, newSchema);

                // 컬럼이 변경된 경우, 데이터 파일도 변경이 필요.
                if (false == UpdateSchemaRows(oldTB, newTB, newSchema, oldSchema.Columns, newSchema.Columns))
                {
                    return false;
                }

                // Update type buidler.
                SetTypeBuilder(newSchema.Name, newTB);
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
        private bool UpdateSchemaRows(TypeBuilder oldTB, TypeBuilder newTB, ClassSchema newSchema, List<ClassSchemaColumn> oldCols, List<ClassSchemaColumn> newCols)
        {
            var addCols = GetAddCols(newCols);
            var updateCols = GetUpdateCols(oldCols, newCols);

            // Get rows.
            var oriRows = GetRows(newSchema.Name);
            if (default == oriRows)
            {
                return false;
            }

            // Create List for new type.
            var classListType = typeof(List<>).MakeGenericType(newTB.CreateType());
            var classList = (IList)Activator.CreateInstance(classListType);

            var oldProps = oldTB.CreateType().GetProperties();
            var newProps = newTB.CreateType().GetProperties();

            // Search original row.
            foreach (var oriRow in oriRows)
            {
                var type = newTB.CreateType();

                // Search new props.
                foreach (var prop in newProps)
                {
                    var typeProp = type.GetProperty(prop.Name);
                    object value = default;

                    // Input column data.
                    if (addCols.Exists(d => d.Equals(prop.Name)))
                    {
                        // Add column.
                        typeProp.SetValue(type, default);
                    }
                    else if (updateCols.ContainsKey(prop.Name))
                    {
                        // Update column.
                        var oldProp = oldProps.FirstOrDefault(d => d.Name.Equals(updateCols[prop.Name]));
                        value = oldProp.GetValue(oriRow, null);
                    }
                    else
                    {
                        // Old colmn.
                        value = prop.GetValue(oriRow, null);
                    }

                    // Old column.
                    typeProp.SetValue(type, value);
                }

                classList.Add(type);
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
                var oldCol = oldCols.First(d => d.Idx.Equals(newCol.Idx));
                if (null != oldCol)
                {
                    colDict.Add(oldCol.Name, newCol.Name);
                }
            }

            return colDict;
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
