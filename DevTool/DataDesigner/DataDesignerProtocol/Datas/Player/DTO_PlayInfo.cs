using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.Datas.Play
{
    using GameCore.Datas.Plays;

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
            Stat = new DTO_Stat();
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
        /// 장비.
        /// </summary>
        [MemoryPackOrder(3)]
        public DTO_Equipment Equipment { get; set; }

        /// <summary>
        /// 능력치.
        /// </summary>
        [MemoryPackOrder(4)]
        public DTO_Stat Stat { get; set; }

        /// <summary>
        /// regtime.
        /// </summary>
        [MemoryPackOrder(5)]
        public DateTime Regtime { get; set; }
    }
}
