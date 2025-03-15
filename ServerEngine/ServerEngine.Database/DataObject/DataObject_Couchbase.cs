using Couchbase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.DataObject
{
    using Newtonsoft.Json.Linq;
    using ServerEngine.Config.Database;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using System.Text;

    public class DataObject_Couchbase : IDataObject
    {
        private readonly ILogger<DataObject_Couchbase> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly IJsonSerializer _jsonSerializer;

        // Account information.
        private readonly string _host;
        private readonly string _account;
        private readonly string _password;
        
        // Queries.
        private readonly string _selectQuery;
        private string _upsertQuery;

        private byte[] _oriDataBytes;

        public DataObject_Couchbase(IServiceProvider serviceProvider, Config_Couchbase configCouchbase, Config_CouchbaseConnection connection)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DataObject_Couchbase>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            // Couchbase.
            _host = configCouchbase.Host;
            _account = configCouchbase.User;
            _password = configCouchbase.Pass;

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
                using (var cluster = await Cluster.ConnectAsync(_host, _account, _password))
                {
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

                    return await result.FirstAsync();
                }
            }
            catch
            {
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
                using (var cluster = await Cluster.ConnectAsync(_host, _account, _password))
                {
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
            }
            catch
            {
                return false;
            }
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
