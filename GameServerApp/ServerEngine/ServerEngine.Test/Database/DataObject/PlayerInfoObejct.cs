
namespace ServerEngine.Test.Database.DataObject
{
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Core.DataObject;
    using ServerEngine.Database.DataObject;
    using ServerEngine.Test.Database.Data;

    public class PlayerInfoObejct : DataObject_PSQL
    {
        private readonly ILogger<PlayerInfoObejct> _logger;

        public PlayerInfoObejct(IServiceProvider serviceProvider)
            : base (
                  serviceProvider,
                  new DataObjectInfo
                  {
                      Document = "Game",
                      Bucket = "TBL_PlayerInfo"
                  },
                  GameConfigManager.GetDBConnectString("Game"))
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PlayerInfoObejct>>();
        }

        /// <summary>
        /// Select.
        /// </summary>s
        public new T Select<T>(string pid) where T : DTO_PlayerInfo
        {
            var playerInfo = base.Select<T>(pid);
            if (null == playerInfo)
            {
                _logger.LogError("failed to select PlayerInfo.");
                return null;
            }
            
            return playerInfo;
        }


        /// <summary>
        /// Upsert.
        /// </summary>
        public new bool Upsert<T>(string pid, T dataObject) where T : DTO_PlayerInfo
        {
            var isUpsert = base.Upsert(pid, dataObject);
            if (false == isUpsert)
            {
                _logger.LogError("failed to upsert PlayerInfo.");
                return false;
            }

            return false;
        }
    }
}