
namespace ServerEngine.Database.Interfaces
{
    /// <summary>
    /// IDBObject.
    /// </summary>
    public interface IDBObject
    {
        /// <summary>
        /// Get blob data.
        /// </summary>
        Task<T> SelectAsync<T>(string key) where T : class, new();

        /// <summary>
        /// Update blob data.
        /// </summary>
        Task<bool> UpsertAsync<T>(string key, T dataObject) where T : class, new();
    }
}
