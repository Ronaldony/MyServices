
using ServerEngine.Database.DataObject;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using GameCore.Datas.Player;

    /// <summary>
    /// DBPostInfo.
    /// </summary>
    public class DBPostInfo : DBObject_Couchbase
    {
        public DBPostInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager.ConfigGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("PostInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<DTO_PostInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<DTO_PostInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, DTO_PostInfo dataObject)
        {
            return  await base.UpsertAsync(pid, dataObject);
        }
    }
}
