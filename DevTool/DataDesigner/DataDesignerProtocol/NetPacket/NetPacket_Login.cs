
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacketCS_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_Login : NetPacketBase_CS
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Token { get; set; }
    }

    /// <summary>
    /// NetPacketSC_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_Login : NetPacketBase_SC
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
    }
}
