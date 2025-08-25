
namespace Server_Realm.Entities
{
    using GameCore.Datas.Items;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Root item.
        /// </summary>
        public bool RootItem(long dropId)
        {
            // 드랍할 수 있는 아이템인지 검사.
            if (MobDropItems.ContainsKey(dropId))
            {
                return false;
            }

            var dtoItem = MobDropItems[dropId];

            // 드랍된 아이템 목록에서 제거.
            MobDropItems.Remove(dropId);

            // 아이템 추가.
            if (false == InventoryInfo.Items.ContainsKey(dtoItem.RefId))
            {
                // 아이템 추가
                InventoryInfo.Items.Add(dtoItem.RefId, dtoItem);
            }
            else
            {
                // 개수 증가
                InventoryInfo.Items[dtoItem.RefId].Count += dtoItem.Count;
            }

            return true;
        }

        /// <summary>
        /// Add drop item.
        /// </summary>
        public void AddDropItemList(long dropId, DTO_Item dropItem)
        {

        }
    }
}
    