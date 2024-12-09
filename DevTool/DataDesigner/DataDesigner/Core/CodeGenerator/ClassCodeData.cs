namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// Data for generating class type.
    /// </summary>
    internal sealed class ClassCodeData
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public bool IsIndexed { get; set; }
    }
}
