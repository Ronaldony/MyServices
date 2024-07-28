using ServerEngine.Database.Data;

namespace ServerEngine.Database.Interfaces
{
    public interface IDataObject
    {
        /// <summary>
        /// Get blob data.
        /// </summary>
        T Select<T>(string key) where T : DataObjectBase;

        /// <summary>
        /// Update blob data.
        /// </summary>
        bool Upsert<T>(string key, T dataObject) where T : DataObjectBase;
    }
}
