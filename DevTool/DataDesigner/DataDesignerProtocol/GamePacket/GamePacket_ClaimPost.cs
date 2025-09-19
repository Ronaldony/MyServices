
using GameCore.GamePacket;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// GamePacketCS_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.CS_ClaimPost)]
    public partial class GamePacketCS_ClaimPost : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.SC_ClaimPost)]
    public partial class GamePacketSC_ClaimPost : GamePacketBase_SC
    {
        /// <summary>
        /// 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string PostUid { get; set; }

        public override void Dispose()
        {
        }
    }
}
