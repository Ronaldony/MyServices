
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_Enter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_Enter : GamePacketBase_CS
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Token { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Enter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_Enter : GamePacketBase_SC
    {
        /// <summary>
        /// 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PlayInfo PlayInfo { get; set; }

        /// <summary>
        /// 인벤토리 정보.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_InventoryInfo InventoryInfo { get; set; }
    }
}
