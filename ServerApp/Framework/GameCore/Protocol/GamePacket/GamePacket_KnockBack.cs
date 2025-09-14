
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
        [MemoryPackOrder(2)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// 피격 발생 위치.
        /// </summary>
        [MemoryPackOrder(3)]
        public Vector2 SrcPosition { get; set; }
    }

    /// <summary>
    /// GamePacketSC_KnockBack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_KnockBack)]
    public partial class GamePacketSC_KnockBack : GamePacketBase_SC
    {
        /// <summary>
        /// Unit id.
        /// </summary>
        [MemoryPackOrder(2)]
        public long UnitId { get; set; }

        /// <summary>
        /// 넉백 위치.
        /// </summary>
        [MemoryPackOrder(3)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// 피격 발생 위치.
        /// </summary>
        [MemoryPackOrder(4)]
        public Vector2 SrcPosition { get; set; }

        public override void Dispose()
        {
        }
    }
}
