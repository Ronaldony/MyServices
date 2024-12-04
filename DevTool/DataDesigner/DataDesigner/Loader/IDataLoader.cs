namespace DataDesigner.Loader
{
    /// <summary>
    /// DataLoader interface.
    /// </summary>
    internal interface IDataLoader
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Load datas from files.
        /// </summary>
        public void LoadAllDatas();

        /// <summary>
        /// Get data dictionary.
        /// </summary>
        IDictionary<string, string> GetDataDict();
    }
}
