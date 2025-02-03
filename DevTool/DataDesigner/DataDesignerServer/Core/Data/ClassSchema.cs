namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// ClassSchema.
    /// </summary>
    public class ClassSchema
    {
        public ClassSchema()
        {
            Columns = new List<ClassSchemaColumn>();
        }

        /// <summary>
        /// Schema name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Schema description.
        /// </summary>
        public string Desciption { get; set; }

        /// <summary>
        /// Columns.
        /// </summary>
        public List<ClassSchemaColumn> Columns { get; set; }
    }
}
