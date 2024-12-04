namespace DataDesigner.Core.DataGenerator
{
    /// <summary>
    /// Interface for data generator.
    /// </summary>
    internal interface IDataGenerator
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Generate data.
        /// </summary>
        bool GenerateData<T>(T data);
    }
}
