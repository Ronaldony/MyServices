
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacket_LoadPlayer from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_LoadPlayer
    {
    }

    /// <summary>
    /// NetPacket_LoadPlayer from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_LoadPlayer : NetPacketResBase
    {
        /// <summary>
        /// 유저 플레이 정보.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_PlayInfo> Users { get; set; }
    }
}
