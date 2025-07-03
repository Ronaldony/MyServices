using GameCore.Datas.Play;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Equipment from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_ChangeEquipment : NetPacketBase_CS
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PlayInfo PlayInfo { get; set; }
    }

    /// <summary>
    /// NetPacket_Equipment from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_ChangeEquipment
    {
    }
}
