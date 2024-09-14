
namespace ServerEngine.Database.Interfaces
{
    using ServerEngine.Database.Data;

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
