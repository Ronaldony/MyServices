namespace DataDesigner.Core.Data
{
    /// <summary>
    /// ClassSchemaColumn.
    /// </summary>
    public sealed class ClassSchemaColumn
    {
        /// <summary>
        /// Column index.
        /// </summary>
        public int Idx { get; set; }

        /// <summary>
        /// Column Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Index status.
        /// </summary>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Reference enum.
        /// </summary>
        public Type RefEnum { get; set; }

        /// <summary>
        /// Reference class.
        /// </summary>
        public Type RefClass { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }
    }
}
