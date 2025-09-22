
using Couchbase.Client.Transactions;
using Couchbase.Client.Transactions.Config;

namespace ServerEngine.Database.DBObject.Transactions
{
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// CouchbaseTransaction.
    /// </summary>
    public class CouchbaseTransaction : TransactionContext
    {
        public AttemptContext Context { get; set; }

        /// <summary>
        /// Upsert for async.
        /// </summary>
        public override async Task<bool> UpsertAsync<T>(IDBObject dbObject, string key, T data)
        {
            if (Context == null)
            {
                return false;
            }

            var result = await Context.QueryAsync<dynamic>(
                dbObject.GetQuery_Upsert(),
                options => options
                    .AdHoc(false)           // Query caching.
                    .Parameter("key", key)
                    .Parameter("value", data)
                    );

            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                // Fail.
                return false;
            }

            return true;
        }
    }
}
