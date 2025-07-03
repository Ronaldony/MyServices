using MemoryPack;
using System;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Enemy
{
    /// <summary>
    /// DTO_Enemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Enemy
    {
        public DTO_Enemy()
        {
            Location = new DTO_EnemyLocation();
        }

        /// <summary>
        /// 적 고유 Id.
        /// </summary>
        [MemoryPackOrder(0)]
        public ulong EnemeyUid { get; set; }

        /// <summary>
        /// 적 Id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int EnemyId { get; set; }

        /// <summary>
        /// 체력.
        /// </summary>
        [MemoryPackOrder(2)]
        public int Health { get; set; }

        /// <summary>
        /// 업데이트 시간.
        /// </summary>
        [MemoryPackOrder(3)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 업데이트 시간.
        /// </summary>
        [MemoryPackOrder(4)]
        public DateTime HitTime { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        [MemoryPackOrder(5)]
        public DTO_EnemyLocation Location { get; set; }

        /// <summary>
        /// 드랍 아이템.
        /// </summary>
        [MemoryPackOrder(6)]
        public Type_Item DropItem { get; set; }

        /// <summary>
        /// 행동 유형.
        /// </summary>
        [MemoryPackOrder(7)]
        public Type_Action ActionType { get; set; }
    }
}
