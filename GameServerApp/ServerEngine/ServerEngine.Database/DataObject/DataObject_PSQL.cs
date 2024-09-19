using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;

namespace ServerEngine.Database.DataObject
{
	using ServerEngine.Core.Services.Interfaces;
	using ServerEngine.Database.Interfaces;

	/// <summary>
	/// Data object for PostgreSQL.
	/// </summary>
	public abstract class DataObject_PSQL : IDataObject
    {
        private readonly ILogger<DataObject_PSQL> _logger;
        
        private readonly IDataSerializer _dataSerializer;
        private IMemcachedService _cacheService;

        private DataObjectInfo _dataObjectInfo;
        private readonly string _connectString;

        private byte[] _oriDataBytes;

        public DataObject_PSQL(IServiceProvider serviceProvider, DataObjectInfo dataObjectInfo, string connectString)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DataObject_PSQL>>();

            _cacheService = serviceProvider.GetRequiredService<IMemcachedService>();
			_dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _dataObjectInfo = dataObjectInfo;

			// set connection string.
			var connectionSB = new NpgsqlConnectionStringBuilder(connectString);
            connectionSB.Database = _dataObjectInfo.Document;
            _connectString = connectionSB.ConnectionString;
        }

        /// <summary>
        /// Select.
        /// </summary>
        public T Select<T>(string key) where T : DataObjectBase
        {
            // Get from memory cache.
            var cachingData = _cacheService.Get<T>(key);
            if (null != cachingData)
            {
                return cachingData;
			}

            // Select Database.
            try
            {
                var sql = $"""
                        SELECT _data FROM "{_dataObjectInfo.Bucket}" WHERE key=@Key
                        """;

                using (var connection = new NpgsqlConnection(_connectString))
                {
                    connection.Open();

                    var dataBytes = connection.QueryFirstOrDefault<byte[]>(
                        sql: sql,
                        param: new
                        {
                            Key = key,
                        },
                        commandTimeout: 10,
                        commandType: CommandType.Text
                        );

                    // save ori data bytes.
                    _oriDataBytes = dataBytes;

                    var dataObject = _dataSerializer.Deserialize<T>(dataBytes);

					// update cache.
					_cacheService.Set<T>(key, dataObject);

					return dataObject;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Upsert.
        /// </summary>
        public bool Upsert<T>(string key, T dataObject) where T : DataObjectBase
        {
            var dataBytes = _dataSerializer.Serialize(dataObject);

            // dirty check.
            if (false == IsDirty(_oriDataBytes, dataBytes))
            {
                // skip upsert DB.
                return true;
            }

            // update cache.
            _cacheService.Set<T>(key, dataObject);

            // Upsert.
            try
            {
               var sql = $"""
                        INSERT INTO "{_dataObjectInfo.Bucket}" (key, _data, regtime) VALUES (@Key, @Data, NOW()) 
                        ON CONFLICT (key) 
                        DO UPDATE 
                        SET _data=@Data
                        """;

                using (var connection = new NpgsqlConnection(_connectString))
                {
                    // Connection.
                    connection.Open();

                    int rowAffect = connection.Execute(
                        sql: sql,
                        param: new
                        {
                            Key = key,
                            Data = dataBytes
                        },
                        commandTimeout: 10,
                        commandType: CommandType.Text
                        );

                    // Upsert success.
                    if (rowAffect > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Dirty check for obejct bytes.
        /// </summary>
        private bool IsDirty(byte[] ori, byte[] dst)
        {
            if ((null == ori) || (null == dst))
			{
                return true;
            }

            if (false == ori.Length.Equals(dst.Length))
            {
                return true;
            }

            var longSize = ori.Length / sizeof(long);
            var oriLong = Unsafe.As<long[]>(ori);
            var dstLong = Unsafe.As<long[]>(dst);
            
            for (int i = 0; i < longSize; i++)
            {
                if (false == oriLong[i].Equals(dstLong[i]))
                {
                    return true;
                }
            }

            for (int i = longSize * 8; i < ori.Length; i++)
            {
                if (false == ori[i].Equals(dst[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
