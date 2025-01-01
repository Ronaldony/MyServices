namespace DataDesigner.Core.Data
{
    /// <summary>
    /// EnumSchema.
    /// </summary>
    public sealed class EnumSchema
    {
        /// <summary>
        /// Schema name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Columns.
        /// </summary>
        public List<EnumSchemaColumn> Columns { get; set; }
    }
}
