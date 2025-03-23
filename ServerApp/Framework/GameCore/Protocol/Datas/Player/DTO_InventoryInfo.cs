using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Inventory
{
    using GameCore.Datas.Player.Inventory;

    /// <summary>
    /// 인벤토리 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_InventoryInfo : DTO_InfoBase
    {
        public DTO_InventoryInfo()
        {
            ItemSlots = new List<DTO_ItemSlot>();
        }

        /// <summary>
        /// Item slots.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_ItemSlot> ItemSlots { get; set; }
    }
}
