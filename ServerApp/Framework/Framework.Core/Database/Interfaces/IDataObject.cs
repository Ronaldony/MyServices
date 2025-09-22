using ServerEngine.Database.DBObject.Transactions;
using ServerEngine.Database.Interfaces;

namespace Framework.Core.Database.Interfaces
{
    /// <summary>
    /// IDataObject.
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        /// Load data async.
        /// </summary>
        Task LoadDataAsync(string pid);

        /// <summary>
        /// Upsert async.
        /// </summary>
        Task<bool> UpsertAsync(string pid);

        /// <summary>
        /// Upsert async for async.
        /// </summary>
        Task<bool> UpsertAsync(TransactionContext tranContext, string pid);

        /// <summary>
        /// Get DB object.
        /// </summary>
        IDBObject GetDBObject();
    }
}
