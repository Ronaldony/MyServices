namespace ServerEngine.Database.Interfaces
{
    public interface IDataObject
    {
        /// <summary>
        /// Get blob data.
        /// </summary>
        T Select<T>(string key) where T : class;

        /// <summary>
        /// Update blob data.
        /// </summary>
        bool Upsert<T>(string key, T data) where T : class;
    }
}
