
using ServerEngine.Database.DataObject;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using GameCore.Datas.Play;

    /// <summary>
    /// DBPlayInfo.
    /// </summary>
    public class DBPlayInfo : DBObject_Couchbase
    {
        public DBPlayInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager.ConfigGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("PlayInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<DTO_PlayInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<DTO_PlayInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, DTO_PlayInfo dataObject)
        {
            return  await base.UpsertAsync(pid, dataObject);
        }
    }
}
