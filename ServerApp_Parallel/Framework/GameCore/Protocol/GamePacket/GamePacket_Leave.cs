using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_Leave.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_Leave)]
    public partial class GamePacketCS_Leave : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_Leave.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_Leave)]
    public partial class GamePacketSC_Leave : GamePacketBase_SC
    {
    }
}
