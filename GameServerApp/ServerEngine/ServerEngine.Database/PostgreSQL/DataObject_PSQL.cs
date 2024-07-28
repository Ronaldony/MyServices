using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace ServerEngine.Database.PostgreSQL
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Data;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Database.Types;

    public abstract class DataObject_PSQL : IDataObject
    {
        private readonly ILogger<DataObject_PSQL> _logger;
        private readonly IDataSerializer _dataSerializer;

        private DataObjectInfo _dataObjectInfo;
        private readonly string _connectString;

        public DataObject_PSQL(IServiceProvider serviceProvider, Type_DataObject dataObjectType, string connectString)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DataObject_PSQL>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            // get DataObjectInfo.
            var dataObjectService = serviceProvider.GetRequiredService<IDataObjectService>();
            _dataObjectInfo = dataObjectService.GetDataObjectInfo(dataObjectType);

            // set connection string.
            var connectionSB = new NpgsqlConnectionStringBuilder(connectString);
            connectionSB.Database = _dataObjectInfo.Database;
            _connectString = connectionSB.ConnectionString;
        }

        /// <summary>
        /// Select.
        /// </summary>
        public T Select<T>(string key) where T : DataObjectBase
        {
            // TODO: Select from cache first.

            // Select Database.
            try
            {
                var sql = $"""
                        SELECT _data FROM "{_dataObjectInfo.Table}" WHERE key=@Key
                        """;

                using (var connection = new NpgsqlConnection(_connectString))
                {
                    connection.Open();

                    var dataBytes = connection.QueryFirstOrDefault(
                        sql: sql,
                        param: new
                        {
                            Key = key,
                        },
                        commandTimeout: 10,
                        commandType: CommandType.Text
                        );

                    return _dataSerializer.Deserialize<T>(dataBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public bool Upsert<T>(string key, T dataObject) where T : DataObjectBase
        {
            var dataBytes = _dataSerializer.Serialize(dataObject);

            // TODO: Check object is changed.

            // TODO: Update cache.

            // Upsert Database.

            try
            {
               var sql = $"""
                        INSERT INTO "{_dataObjectInfo.Table}" (key, _data, regtime) VALUES (@Key, @Data, NOW()) 
                        ON CONFLICT (key) 
                        DO UPDATE 
                        SET _data=@Data
                        """;

                using (var connection = new NpgsqlConnection(_connectString))
                {
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
    }
}
