
using ServerEngine.Database.DataObject;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using GameCore.Datas.Player;

    /// <summary>
    /// DBPlayInfo.
    /// </summary>
    public class DBPlayerInfo : DBObject_Couchbase
    {
        public DBPlayerInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager.GetCouchbaseConnection("PlayerInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<DTO_PlayerInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<DTO_PlayerInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, DTO_PlayerInfo dataObject)
        {
            return  await base.UpsertAsync(pid, dataObject);
        }
    }
}
