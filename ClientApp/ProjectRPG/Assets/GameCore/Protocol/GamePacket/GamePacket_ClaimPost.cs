
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// GamePacketCS_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.CS_ClaimPost)]
    public partial class GamePacketCS_ClaimPost : HubPacketBase_CS
    {
        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }

    /// <summary>
    /// GamePacketSC_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.SC_ClaimPost)]
    public partial class GamePacketSC_ClaimPost : HubPacketBase_SC
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
