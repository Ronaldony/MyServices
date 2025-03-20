using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Play
{
    using GameCore.Datas.Player.Inventory;
    using GameCore.Datas.Player.Status;

    /// <summary>
    /// 플레이 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayInfo
    {
        public DTO_PlayInfo()
        {
            Equipment = new DTO_Equipment();
            Status = new DTO_PlayerStat();
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
        /// Area.
        /// </summary>
        [MemoryPackOrder(3)]
        public Type_Area Area  { get; set; }

        /// <summary>
        /// Pos.
        /// </summary>
        [MemoryPackOrder(4)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// 장비.
        /// </summary>
        [MemoryPackOrder(5)]
        public DTO_Equipment Equipment { get; set; }

        /// <summary>
        /// 플레이어 능력치.
        /// </summary>
        [MemoryPackOrder(6)]
        public DTO_PlayerStat Status { get; set; }
    }
}
