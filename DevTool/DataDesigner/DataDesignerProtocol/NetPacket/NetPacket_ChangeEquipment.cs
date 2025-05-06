using GameCore.Datas.Play;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Equipment from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_ChangeEquipment : NetPacketReqBase
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_PlayInfo PlayInfo { get; set; }
    }

    /// <summary>
    /// NetPacket_Equipment from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_ChangeEquipment
    {
    }
}
