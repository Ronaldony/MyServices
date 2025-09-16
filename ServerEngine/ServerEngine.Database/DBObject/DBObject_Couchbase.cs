using Couchbase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.DataObject
{
    using ServerEngine.Config.Database;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;

    public abstract class DBObject_Couchbase : IDBObject
    {
        private readonly ILogger<DBObject_Couchbase> _logger;

        private readonly IJsonSerializer _jsonSerializer;

        // Connection.
        private readonly IDBObjectConn _conn;

        // Queries.
        private readonly string _selectQuery;
        private string _upsertQuery;

        public DBObject_Couchbase(IServiceProvider serviceProvider, Config_CouchbaseConnection connection)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DBObject_Couchbase>>();

            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _conn = serviceProvider.GetRequiredService<IDBObjectConn>();

            _selectQuery = $"SELECT c.* FROM `{connection.Bucket}`.{connection.Scope}.{connection.Collection} c USE KEYS $key";
            _upsertQuery = $"UPSERT INTO `{connection.Bucket}`.{connection.Scope}.{connection.Collection} (KEY, VALUE) VALUES ($key, $value)";
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<T> SelectAsync<T>(string key) where T : class, new()
        {
            try
            {
                var cluster = _conn.GetConnection<ICluster>();

                // Query.
                var result = await cluster.QueryAsync<T>(
                    _selectQuery,
                    options => options
                        .AdHoc(true)           // Query caching.
                        .Parameter("key", key)
                        );

                if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
                {
                    // Fail.
                    return null;
                }

                return await result.FirstAsync();
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
        public async Task<bool> UpsertAsync<T>(string key, T dataObject) where T : class, new()
        {
            try
            {
                var cluster = _conn.GetConnection<ICluster>();

                var result = await cluster.QueryAsync<dynamic>(
                    _upsertQuery,
                    options => options
                        .AdHoc(true)           // Query caching.
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
    }
}
