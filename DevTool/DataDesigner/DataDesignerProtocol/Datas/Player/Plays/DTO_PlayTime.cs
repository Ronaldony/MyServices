using MemoryPack;
using System;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Plays
{
    /// <summary>
    /// DTO_PlayTime.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayTime
    {
        public DTO_PlayTime()
        {

        }

        /// <summary>
        /// Last packet time.
        /// </summary>
        [MemoryPackOrder(0)]
        public DateTime LastPacketTime { get; set; }

        /// <summary>
        /// Last attack time.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime LastAttackTime { get; set; }

        /// <summary>
        /// Last hit time.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime LastHitTime { get; set; }
    }
}
