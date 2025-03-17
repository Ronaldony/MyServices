
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.Core.Database.DataObject
{
    using GameCore.Datas.Player;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Core.Util;
    using ServerEngine.Database.DataObject;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class PlayerInfoObject : DataObject_Couchbase
    {
        private readonly ILogger<PlayerInfoObject> _logger;

        public PlayerInfoObject(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("PlayerInfo"))
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PlayerInfoObject>>();
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

        /// <summary>
        /// Create object.
        /// </summary>
        public DTO_PlayerInfo CreateObject()
        {
            return new DTO_PlayerInfo
            {
                RegTime = TimeUtil.Now
            };
        }
    }
}
