using Couchbase;
using Couchbase.Client.Transactions.Config;
using Couchbase.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.DataObject
{
    using ServerEngine.Config.Database;
    using ServerEngine.Database.DBObject.Connections;
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// DBObject_Couchbase.
    /// </summary>
    public abstract class DBObject_Couchbase : IDBObject
    {
        private readonly ILogger<DBObject_Couchbase> _logger;

        // Connection.
        private readonly CouchbaseConnection _conn;

        // Queries.
        private readonly string _selectQuery;
        private readonly string _upsertQuery;

        public DBObject_Couchbase(IServiceProvider serviceProvider, Config_CouchbaseConnection connection)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DBObject_Couchbase>>();

            _conn = serviceProvider.GetRequiredService<CouchbaseConnection>();

            _selectQuery = $"SELECT c.* FROM `{connection.Bucket}`.{connection.Scope}.{connection.Collection} c USE KEYS $key";
            _upsertQuery = $"UPSERT INTO `{connection.Bucket}`.{connection.Scope}.{connection.Collection} (KEY, VALUE) VALUES ($key, $value)";
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public virtual async Task<T> SelectAsync<T>(string key) where T : class, new()
        {
            try
            {
                var cluster = _conn.GetConnection();

                // Query.
                var result = await cluster.QueryAsync<T>(
                    _selectQuery,
                    options => options
                        .AdHoc(false)           // Query caching.
                        .Parameter("key", key)
                        );

                if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
                {
                    // Fail.
                    return null;
                }

                return await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public virtual async Task<bool> UpsertAsync<T>(string key, T dataObject) where T : class, new()
        {
            try
            {
                var cluster = _conn.GetConnection();

                var result = await cluster.QueryAsync<dynamic>(
                    _upsertQuery,
                    options => options
                        .AdHoc(false)           // Query caching.
                        .Parameter("key", key)
                        .Parameter("value", dataObject)
                        );

                if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
                {
                    // Fail.
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Get select query.
        /// </summary>
        public string GetQuery_Select()
        {
            return _selectQuery;
        }

        /// <summary>
        /// Get upsert query.
        /// </summary>
        public string GetQuery_Upsert()
        {
            return _upsertQuery;
        }
    }
}
