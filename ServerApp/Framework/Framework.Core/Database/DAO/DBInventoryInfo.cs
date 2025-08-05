
using ServerEngine.Database.DataObject;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using GameCore.Datas.Inventory;

    /// <summary>
    /// DBInventoryInfo.
    /// </summary>
    public class DBInventoryInfo : DBObject_Couchbase
    {
        public DBInventoryInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager.ConfigGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("InventoryInfo"))
        {
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<DTO_InventoryInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<DTO_InventoryInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, DTO_InventoryInfo dataObject)
        {
            return  await base.UpsertAsync(pid, dataObject);
        }
    }
}
