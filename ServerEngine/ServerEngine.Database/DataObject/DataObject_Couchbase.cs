
namespace ServerEngine.Database.DataObject
{
    using Couchbase;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Npgsql;
    using ServerEngine.Core.DataObject;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using System.Threading.Tasks;

    public class DataObject_Couchbase : IDataObject
    {
        private readonly ILogger<DataObject_Couchbase> _logger;

        private readonly IDataSerializer _dataSerializer;

        private DataObjectInfo _dataObjectInfo;

        private ICluster _cluster;

        private byte[] _oriDataBytes;

        public DataObject_Couchbase(IServiceProvider serviceProvider, DataObjectInfo dataObjectInfo)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DataObject_Couchbase>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _dataObjectInfo = dataObjectInfo;
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<T> SelectAsync<T>(string key) where T : class, new()
        {
            _cluster = await Cluster.ConnectAsync(
                // Update these credentials for your Local Couchbase instance!
                "couchbase://your-ip",
                "Administrator",
                "password");

            return default;
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync<T>(string key, T dataObject) where T : class, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implimented.
        /// </summary>
        public T Select<T>(string key) where T : class, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implimented.
        /// </summary>
        public bool Upsert<T>(string key, T dataObject) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
