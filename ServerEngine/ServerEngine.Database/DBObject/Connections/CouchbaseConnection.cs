using Couchbase;
using Couchbase.Client.Transactions;
using Couchbase.Client.Transactions.Config;
using Couchbase.KeyValue;

namespace ServerEngine.Database.DBObject.Connections
{
    using ServerEngine.Config.Database;
    using ServerEngine.Database.DBObject.Transactions;

    /// <summary>
    /// CouchbaseConnection.
    /// </summary>
    public class CouchbaseConnection
    {
        private ICluster _cluster;

        public CouchbaseConnection()
        {
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public async Task InitializeAsync(Config_Couchbase config, DurabilityLevel durabilityLevel)
        {
            var transactionsConfig = TransactionsConfigBuilder.Create()
                .DurabilityLevel(durabilityLevel)
                .Build();

            var options = new ClusterOptions()
            {
                ConnectionString = config.Host,
                UserName = config.User,
                Password = config.Pass,
                TransactionsConfig = transactionsConfig,
            };

            _cluster = await Cluster.ConnectAsync(options);
        }

        /// <summary>
        /// Get connection.
        /// </summary>
        public ICluster GetConnection()
        {
            return _cluster;
        }

        /// <summary>
        /// Run transactions.
        /// </summary>
        public async Task<TransactionResult> TransactionAsync(Func<TransactionContext, Task> task)
        {
            var ts = _cluster.Transactions;

            return await ts.RunAsync(async (attemptContext) =>
            {
                // context.
                var context = new CouchbaseTransaction();
                context.Context = attemptContext;

                await task(context);
            });
        }
    }
}
