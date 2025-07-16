using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Plays
{
    /// <summary>
    /// 플레이어 능력치.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerStat
    {
        /// <summary>
        /// 최대 체력.
        /// </summary>
        [MemoryPackOrder(0)]
        public int HealthMax { get; set; }

        /// <summary>
        /// 현재 체력.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Health { get; set; }

        /// <summary>
        /// 공격력.
        /// </summary>
        [MemoryPackOrder(2)]
        public int AttackPower { get; set; }

        /// <summary>
        /// 방어력.
        /// </summary>
        [MemoryPackOrder(3)]
        public int Defense { get; set; }

        /// <summary>
        /// 속도.
        /// </summary>
        [MemoryPackOrder(4)]
        public int Velocity { get; set; }
    }
}
