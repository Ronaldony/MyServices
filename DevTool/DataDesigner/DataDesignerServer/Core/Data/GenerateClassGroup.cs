using DataDesignerServer.Core.Types;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// Type class group.
    /// </summary>
    public class GenerateClassGroup
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }

        /// <summary>
        /// Enum builder.
        /// </summary>
        public TypeBuilder Builder { get; set; }

        /// <summary>
        /// Row Datas.
        /// </summary>
        public List<object> Rows { get; set; }

        /// <summary>
        /// ClassDataType.
        /// </summary>
        public Type_ClassData ClassDataType { get; set; }
    }
}
