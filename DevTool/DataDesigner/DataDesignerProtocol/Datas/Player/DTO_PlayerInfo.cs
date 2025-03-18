using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player
{
    using GameCore.Datas.Inventory;

    /// <summary>
    /// DTO_PlayerInfo.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerInfo
    {
        public DTO_PlayerInfo()
        {
            InventoryInfo = new DTO_InventoryInfo();
        }

        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Pid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [MemoryPackOrder(2)]
        public string PlayerName { get; set; }

        /// <summary>
        /// InventoryInfo.
        /// </summary>
        [MemoryPackOrder(3)]
        public DTO_InventoryInfo InventoryInfo { get; set; }

        /// <summary>
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(4)]
        public DateTime RegTime { get; set; }
    }
}
