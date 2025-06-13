
namespace ServerEngine.Test.Database.DataObject
{
    using GameCore.Datas.Player;
    using ServerEngine.Config.Database;
    using ServerEngine.Database.DataObject;

    public class PlayerInfoObejct : DBObject_Couchbase
    {
        private readonly ILogger<PlayerInfoObejct> _logger;

        public PlayerInfoObejct(IServiceProvider serviceProvider, Config_Couchbase configCouchbase, Config_CouchbaseConnection connection)
            : base(
                  serviceProvider,
                  configCouchbase: configCouchbase,
                  connection: connection
                  )
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PlayerInfoObejct>>();
        }

        /// <summary>
        /// Select async.
        /// </summary>s
        public new async Task<T> SelectAsync<T>(string pid) where T : DTO_PlayerInfo, new()
        {
            var playerInfo = await base.SelectAsync<T>(pid);

            if (null == playerInfo)
            {
                _logger.LogError("failed to select PlayerInfo.");
                return null;
            }

            return playerInfo;
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public new async Task<bool> UpsertAsync<T>(string pid, T dataObject) where T : DTO_PlayerInfo, new()
        {
            var isUpsert = await base.UpsertAsync(pid, dataObject);

            if (false == isUpsert)
            {
                _logger.LogError("failed to upsert PlayerInfo.");
                return false;
            }

            return false;
        }

    }
}