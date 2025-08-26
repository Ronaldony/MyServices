
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Player;

    /// <summary>
    /// GamePacketCS_EnterPlayDomain.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.CS_EnterPlayDomain)]
    public partial class DomainPacketCS_EnterPlayDomain : DomainPacketBase_CS
    {
        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Token { get; set; }
    }

    /// <summary>
    /// GamePacketSC_EnterPlayDomain.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.SC_EnterPlayDomain)]
    public partial class DomainPacketSC_EnterPlayDomain : DomainPacketBase_SC
    {
        /// <summary>
        /// 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PostInfo PostInfo { get; set; }

        public override void Dispose()
        {
        }
    }
}
