
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_Dash.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_Dash : GamePacketBase_CS
    {
        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(0)]
         public Vector2 Position { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Dash.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_Dash : GamePacketBase_SC
    {
        /// <summary>
        /// 대시 플레이어.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection DashDirection { get; set; }
    }
}
