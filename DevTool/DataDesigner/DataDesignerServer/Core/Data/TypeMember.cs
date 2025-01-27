namespace DataDesignerServer.Core.Data
{
    /// <summary>
    /// Member for type code.
    /// </summary>

    public sealed class TypeMember
    {
        /// <summary>
        /// Member name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Member value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Member comment.
        /// </summary>
        public string Summary { get; set; }
    }
}
