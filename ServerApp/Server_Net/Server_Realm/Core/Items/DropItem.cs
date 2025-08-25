using GameCore.Datas.Items;

namespace Server_Realm.Core.Items
{
    /// <summary>
    /// DropItem.
    /// </summary>
    public class DropItem
    {
        /// <summary>
        /// Drop id.
        /// </summary>
        public long DropId { get; set; }

        /// <summary>
        /// Drop item.
        /// </summary>
        public DTO_Item Item { get; set; }
    }
}
