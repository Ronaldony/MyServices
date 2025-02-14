using System.Reflection.Emit;

namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// Type generate group.
    /// </summary>
    public class GenerateTypeGroup
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public TypeSchema Schema { get; set; }

        /// <summary>
        /// Enum builder.
        /// </summary>
        public EnumBuilder Builder { get; set; }

        /// <summary>
        /// Row Datas.
        /// </summary>
        public List<TypeSchemaRow> Rows { get; set; }
    }
}
