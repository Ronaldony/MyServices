using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace ServerEngine.Database.PostgreSQL
{
    using ServerEngine.Database.Data;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Database.Types;

    public abstract class DataObject_PSQL : IDataObject
    {
        private readonly ILogger<DataObject_PSQL> _logger;
        private DataObjectInfo _dataObjectInfo;

        private readonly string _connectString;

        public DataObject_PSQL(IServiceProvider serviceProvider, Type_DataObject dataObjectType, string connectString)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DataObject_PSQL>();
            
            // Get Data Obejct Info.
            var dataObjectService = serviceProvider.GetRequiredService<IDataObjectService>();
            _dataObjectInfo = dataObjectService.GetDataObjectInfo(dataObjectType);

            // Get connection string.
            var connectionSB = new NpgsqlConnectionStringBuilder(connectString);
            connectionSB.Database = _dataObjectInfo.Database;
            _connectString = connectionSB.ConnectionString;
        }

        /// <summary>
        /// Select 
        /// </summary>
        public T Select<T>(string key) where T : class
        {
            // TODO: Select from cache first.

            // TODO: Select from DB.
            var sql = $"SELECT _data FROM {_dataObjectInfo.Table} WHERE key=@Key";
            using (var connection = new NpgsqlConnection(_connectString))
            {
                connection.Open();

                return connection.QueryFirstOrDefault(
                    sql: sql,
                    param: new
                    {
                        Key = key,
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }

        public bool Upsert<T>(string key, T data) where T : class
        {
            // TODO: Check object is changed.

            // TODO: Update cache.

            // TODO: Upsert from DB.
            var sql = $"""
                        INSERT INTO {_dataObjectInfo.Table} VALUES (@Key, @Data, NOW()) 
                        ON DUPLICATE KEY UPDATE _data=@Data
                        """;

            using (var connection = new NpgsqlConnection(_connectString))
            {
                connection.Open();

                int rowAffect = connection.Execute(
                    sql: sql,
                    param: new
                    {
                        Key = key,
                        Data = data
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );

                // Upsert success.
                if (rowAffect > 0 )
                {
                    return true;
                }

                return false;
            }
        }
    }
}
