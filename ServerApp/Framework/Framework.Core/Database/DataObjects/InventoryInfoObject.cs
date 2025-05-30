namespace Framework.Core.Database.DataObject
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.Attributes;
    using GameCore.Datas.Inventory;
    using ServerEngine.Database.DataObject;

    /// <summary>
    /// InventoryInfoObject.
    /// </summary>
    [DataInfoObject(typeof(DTO_InventoryInfo))]
    public class InventoryInfoObject : DataObject_Couchbase
    {
        public InventoryInfoObject(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("InventoryInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>s
        public new async Task<T> SelectAsync<T>(string pid) where T : DTO_InventoryInfo, new()
        {
            var playInfo = await base.SelectAsync<T>(pid);

            if (null == playInfo)
            {
                return null;
            }

            return playInfo;
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public new async Task<bool> UpsertAsync<T>(string pid, T dataObject) where T : DTO_InventoryInfo, new()
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
