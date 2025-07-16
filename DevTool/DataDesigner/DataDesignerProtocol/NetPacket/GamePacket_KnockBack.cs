
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_KnockBack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_KnockBack : GamePacketBase_CS
    {
        /// <summary>
        /// 넉백 위치.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Position { get; set; }
    }
}
