
using ServerEngine.Database.DataObject;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using Framwork.Core.Session;

    /// <summary>
    /// DBSessionInfo.
    /// </summary>
    public class DBSessionInfo : DBObject_Couchbase
    {
        public DBSessionInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("SessionInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<SessionInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<SessionInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, SessionInfo dataObject)
        {
            return  await base.UpsertAsync(pid, dataObject);
        }
    }
}
