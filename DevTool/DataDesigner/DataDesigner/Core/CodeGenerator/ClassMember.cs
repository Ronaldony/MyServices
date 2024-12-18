namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// Member for class code.
    /// </summary>
    internal sealed class ClassMember
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public bool IsIndexed { get; set; }
    }
}
