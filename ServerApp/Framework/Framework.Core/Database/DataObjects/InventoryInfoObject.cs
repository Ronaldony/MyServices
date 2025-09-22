using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Database.Interfaces;

namespace Framework.Core.Database.DataObjects
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Inventory;
    using ServerEngine.Database.DBObject.Transactions;

    /// <summary>
    /// InventoryInfoObject.
    /// </summary>
    public class InventoryInfoObject : IDataObject
    {
        public DTO_InventoryInfo InventoryInfo => _inventoryInfo;

        private DTO_InventoryInfo _inventoryInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        public InventoryInfoObject(IServiceProvider serviceProvider)
        {
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();
        }

        public async Task LoadDataAsync(string pid)
        {
            _inventoryInfo = await _dbInventoryInfo.SelectAsync(pid);

            if (null == _inventoryInfo)
            {
                _inventoryInfo = new DTO_InventoryInfo();
            }
        }

        public async Task<bool> UpsertAsync(string pid)
        {
            return await _dbInventoryInfo.UpsertAsync<DTO_InventoryInfo>(pid, _inventoryInfo);
        }

        /// <summary>
        /// Get DB object.
        /// </summary>
        public IDBObject GetDBObject()
        {
            return _dbInventoryInfo;
        }

        /// <summary>
        /// Upsert async for transaction.
        /// </summary>
        public async Task<bool> UpsertAsync(TransactionContext tranContext, string pid)
        {
            return await tranContext.UpsertAsync(_dbInventoryInfo, pid, _inventoryInfo);
        }
    }
}
