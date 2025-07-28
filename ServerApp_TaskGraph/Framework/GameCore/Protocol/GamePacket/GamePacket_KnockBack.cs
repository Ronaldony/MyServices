
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_KnockBack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_KnockBack)]
    public partial class GamePacketCS_KnockBack : GamePacketBase_CS
    {
        /// <summary>
        /// 넉백 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }
    }
}
