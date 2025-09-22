
namespace ServerEngine.Database.DBObject.Transactions
{
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// TransactionContext.
    /// </summary>
    public abstract class TransactionContext
    {
        /// <summary>
        /// Upsert for async.
        /// </summary>
        public abstract Task<bool> UpsertAsync<T>(IDBObject dbObject, string key, T data);
    }
}
