
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_Dash.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_Dash)]
    public partial class GamePacketCS_Dash : GamePacketBase_CS
    {
        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
         public Vector2 Position { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Dash.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_Dash)]
    public partial class GamePacketSC_Dash : GamePacketBase_SC
    {
        /// <summary>
        /// 대시 Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Direction { get; set; }
    }
}
