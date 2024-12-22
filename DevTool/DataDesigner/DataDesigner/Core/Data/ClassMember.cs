namespace DataDesigner.Core.DataMember
{
    /// <summary>
    /// Member for class code.
    /// </summary>
    internal sealed class ClassMember
    {
        /// <summary>
        /// Member type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Member name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Member comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Index for DB.
        /// </summary>
        public bool IsIndexed { get; set; }
    }
}
