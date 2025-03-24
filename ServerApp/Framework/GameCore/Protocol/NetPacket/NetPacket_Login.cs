
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacket_Login from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_Login
    {
        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PId { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(2)]
        public string Token { get; set; }
    }

    /// <summary>
    /// NetPacket_Login from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Login
    {
        /// <summary>
        /// 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PlayInfo PlayInfo { get; set; }

        /// <summary>
        /// 인벤토리 정보.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_InventoryInfo InventoryInfo { get; set; }

        /// <summary>
        /// 유저 플레이 정보.
        /// </summary>
        [MemoryPackOrder(2)]
        public List<DTO_PlayInfo> Users { get; set; }

        // 맵 정보 전달 필요.
    }
}
