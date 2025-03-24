namespace Framework.Core.Database.DataObject
{
    using Framwork.Core.Session;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Database.DataObject;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class SessionInfoObject : DataObject_Couchbase
    {
        public SessionInfoObject(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("SessionInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>s
        public new async Task<T> SelectAsync<T>(string pid) where T : SessionInfo, new()
        {
            var playerInfo = await base.SelectAsync<T>(pid);

            if (null == playerInfo)
            {
                return null;
            }

            return playerInfo;
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public new async Task<bool> UpsertAsync<T>(string pid, T dataObject) where T : SessionInfo, new()
        {
            var isUpsert = await base.UpsertAsync(pid, dataObject);

            if (false == isUpsert)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create first object.
        /// </summary>
        protected override T FirstCreateObject<T>()
        {
            throw new NotImplementedException();
        }
    }
}
