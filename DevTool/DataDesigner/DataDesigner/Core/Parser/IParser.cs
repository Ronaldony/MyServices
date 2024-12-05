namespace DataDesigner.Core.Parser
{
    /// <summary>
    /// Interface for parser.
    /// </summary>
    internal interface IParser
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Parse type.
        /// </summary>
        Dictionary<Type, List<Type>> Parse<T>(string typeName, T typeValue);
    }
}
