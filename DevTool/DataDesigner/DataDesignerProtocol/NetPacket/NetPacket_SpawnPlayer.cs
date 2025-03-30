
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacket_SpawnPlayer from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_SpawnPlayer
    {
    }

    /// <summary>
    /// NetPacket_SpawnPlayer from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_SpawnPlayer
    {
        /// <summary>
        /// 유저 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_PlayInfo> Users { get; set; }
    }
}
