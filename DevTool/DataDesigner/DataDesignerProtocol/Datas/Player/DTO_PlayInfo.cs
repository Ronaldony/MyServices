using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.Datas.Play
{
    using GameCore.Datas.Player.Inventory;
    using GameCore.Datas.Player.Status;

    /// <summary>
    /// 플레이 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayInfo : DTO_InfoBase
    {
        public DTO_PlayInfo()
        {
            Position = new DTO_Position();
            Equipment = new DTO_Equipment();
            Stat = new DTO_PlayerStat();
        }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PlayerName { get; set; }

        /// <summary>
        /// Update time for DTO_PlayInfo.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 위치 정보.
        /// </summary>
        [MemoryPackOrder(3)]
        public DTO_Position Position { get; set; }

        /// <summary>
        /// 장비.
        /// </summary>
        [MemoryPackOrder(4)]
        public DTO_Equipment Equipment { get; set; }

        /// <summary>
        /// 플레이어 능력치.
        /// </summary>
        [MemoryPackOrder(5)]
        public DTO_PlayerStat Stat { get; set; }
    }
}
