using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObject
{
    using Framework.Core.Database.Attributes;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Inventory;

    /// <summary>
    /// InventoryInfoObject.
    /// </summary>
    [DataInfoObject(typeof(DTO_InventoryInfo))]
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

        public async Task UpsertAsync(string pid)
        {
            await _dbInventoryInfo.UpsertAsync<DTO_InventoryInfo>(pid, _inventoryInfo);
        }
    }
}
