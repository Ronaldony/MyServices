using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.PostgreSQL.Services
{
    using Npgsql;
    using ServerEngine.Database.Data;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Database.Types;

    public class DataObjectService_PSQL : IDataObjectService
    {
        private readonly ILogger<DataObjectService_PSQL> _logger;
        
        private Dictionary<Type_DataObject, DataObjectInfo> _dataObjectInfos;

        public DataObjectService_PSQL(ILogger<DataObjectService_PSQL> logger)
        {
            _logger = logger;

            // Map Type_BlobData with the class which BlobDataAttribute has.
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _dataObjectInfos = Initialize_DataObjectInfos();

            _logger.LogInformation("DataObjectService_PSQL initialized.");
        }

        /// <summary>
        /// Get table of Type_DataObject.
        /// </summary>
        public DataObjectInfo GetDataObjectInfo(Type_DataObject dataObjectType)
        {
            if (true == _dataObjectInfos.ContainsKey(dataObjectType))
            {
                // exist.
                return _dataObjectInfos[dataObjectType];
            }
            
            return null;
        }

        /// <summary>
        /// Initialize DataObjectInfos.
        /// </summary>
        private Dictionary<Type_DataObject, DataObjectInfo> Initialize_DataObjectInfos()
        {
            var dataObjectInfos = new Dictionary<Type_DataObject, DataObjectInfo>();

            // PlayerInfo.
            dataObjectInfos.Add(
                Type_DataObject.PlayerInfo,
                new DataObjectInfo
                {
                    Database = "Game",
                    Table = "TBL_PlayerInfo"
                });

            return dataObjectInfos;
        }
    }
}
