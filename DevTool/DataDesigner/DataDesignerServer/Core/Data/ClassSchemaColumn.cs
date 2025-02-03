namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// ClassSchemaColumn.
    /// </summary>
    public class ClassSchemaColumn
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
        /// </summary 
        public string RefTable { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Summary { get; set; }
    }
}
