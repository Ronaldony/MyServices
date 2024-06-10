using System.Dynamic;

namespace ServerEngine.Database.Services.Interfaces
{
    public interface IBlobDataService
    {
        /// <summary>
        /// Get blob data by key.
        /// </summary>
        T Select<T>(string key) where T : class;

        /// <summary>
        /// set blob data by key.
        /// </summary>
        bool Update<T>(string key, T data) where T : class;
    }
}
