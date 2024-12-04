namespace DataDesigner.Uploader
{
    internal interface IDataUplaoder
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// 
        /// </summary>
        bool Upload(string path);
    }
}
