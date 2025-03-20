using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player
{
    using GameCore.Datas.Player.Inventory;
    using GameCore.Datas.Player.Status;

    /// <summary>
    /// 플레이어 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerInfo
    {
        public DTO_PlayerInfo()
        {
        }

        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Pid { get; set; }

        /// <summary>
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime RegTime { get; set; }
    }
}
