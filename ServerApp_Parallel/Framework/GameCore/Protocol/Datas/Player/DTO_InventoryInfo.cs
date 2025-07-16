using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Inventory
{
    using GameCore.Datas.Items;

    /// <summary>
    /// 인벤토리 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_InventoryInfo : DTO_InfoBase
    {
        public DTO_InventoryInfo()
        {
            Items = new List<DTO_Item>();
        }

        /// <summary>
        /// Item slots.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Item> Items { get; set; }
    }
}
