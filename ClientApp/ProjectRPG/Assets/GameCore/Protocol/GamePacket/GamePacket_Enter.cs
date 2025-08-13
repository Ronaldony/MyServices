
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
    [GamePacketAttribute(GamePacketCode.CS_Enter)]
    public partial class GamePacketCS_Enter : GamePacketBase_CS
    {
        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(2)]
        public string Token { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Enter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_Enter)]
    public partial class GamePacketSC_Enter : GamePacketBase_SC
    {
        /// <summary>
        /// Enter Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// 플레이 정보.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_PlayInfo PlayInfo { get; set; }

        /// <summary>
        /// 인벤토리 정보.
        /// </summary>
        [MemoryPackOrder(2)]
        public DTO_InventoryInfo InventoryInfo { get; set; }

        public override void Dispose()
        {
        }
    }
}
