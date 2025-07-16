using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.Datas.Play
{
    using GameCore.Datas.Player.Inventory;
    using GameCore.Datas.Player.Plays;

    /// <summary>
    /// 플레이 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayInfo : DTO_InfoBase
    {
        public DTO_PlayInfo()
        {
            Location = new DTO_Location();
            Equipment = new DTO_Equipment();
            Stat = new DTO_PlayerStat();
            PlayTime = new DTO_PlayTime();
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
        /// DTO_Location.
        /// </summary>
        [MemoryPackOrder(2)]
        public DTO_Location Location { get; set; }

        /// <summary>
        /// Mvoe direction.
        /// </summary>
        [MemoryPackOrder(3)]
        public Type_Action Action { get; set; }

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

        /// <summary>
        ///PlayTime.
        /// </summary>
        [MemoryPackOrder(6)]
        public DTO_PlayTime PlayTime { get; set; }

        /// <summary>
        /// regtime.
        /// </summary>
        [MemoryPackOrder(7)]
        public DateTime Regtime { get; set; }
    }
}
