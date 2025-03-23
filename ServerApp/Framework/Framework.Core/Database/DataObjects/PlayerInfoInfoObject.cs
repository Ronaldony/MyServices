namespace Framework.Core.Database.DataObject
{
    using Framework.Core.Database.Attributes;
    using GameCore.Datas.Player;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Database.DataObject;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    [DataInfoObject(typeof(DTO_PlayerInfo))]
    public class PlayerInfoObject : DataObject_Couchbase
    {
        public PlayerInfoObject(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("PlayerInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>s
        public new async Task<T> SelectAsync<T>(string pid) where T : DTO_PlayerInfo, new()
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
        public new async Task<bool> UpsertAsync<T>(string pid, T dataObject) where T : DTO_PlayerInfo, new()
        {
            var isUpsert = await base.UpsertAsync(pid, dataObject);

            if (false == isUpsert)
            {
                return false;
            }

            return false;
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
