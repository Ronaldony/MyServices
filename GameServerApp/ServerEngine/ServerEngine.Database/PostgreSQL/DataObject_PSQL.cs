using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace ServerEngine.Database.PostgreSQL
{
    using ServerEngine.Database.Data;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Database.Types;

    public abstract class DataObject_PSQL : IDataObject
    {
        private readonly ILogger<DataObject_PSQL> _logger;
        private DataObjectInfo _dataObjectInfo;

        public DataObject_PSQL(IServiceProvider serviceProvider, Type_DataObject dataObjectType)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DataObject_PSQL>();
            
            // Get Data Obejct Info.
            var dataObjectService = serviceProvider.GetRequiredService<IDataObjectService>();
            _dataObjectInfo = dataObjectService.GetDataObjectInfo(dataObjectType);
            
            //NpgsqlConnectionStringBuilder
        }

        /// <summary>
        /// Select 
        /// </summary>
        public T Select<T>(string key) where T : class
        {
            // TODO: Select from DB.

            return null;
        }

        public bool Upsert<T>(string key, T data) where T : class
        {
            // TODO: Upsert from DB.

            return true;
        }
    }
}
