using Couchbase;

namespace ServerEngine.Database.DBObject.Connections
{
    using ServerEngine.Config.Database;
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// CouchbaseConnection.
    /// </summary>
    public class CouchbaseConnection : IDBObjectConn
    {
        private readonly Config_Couchbase _config;

        private ICluster _cluster;

        public CouchbaseConnection(Config_Couchbase config)
        {
            _config = config;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public async Task InitializeAsync()
        {
            _cluster = await Cluster.ConnectAsync(_config.Host, _config.User, _config.Pass);
        }

        /// <summary>
        /// Get connection.
        /// </summary>
        public T GetConnection<T>() where T : class
        {
            return _cluster as T;
        }
    }
}
