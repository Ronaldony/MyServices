using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Enemy
{
    using GameCore.Datas.Items;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// DTO_Enemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Enemy
    {
        public DTO_Enemy()
        {
            Location = new DTO_EnemyLocation();
            DropItems = new List<Type_Item>();
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
        /// Track pid.
        /// </summary>
        [MemoryPackOrder(3)]
        public string TrackPid { get; set; }

        /// <summary>
        /// 행동 유형.
        /// </summary>
        [MemoryPackOrder(4)]
        public Type_EnemyAction Action { get; set; }

        /// <summary>
        /// 최근 공격 시간.
        /// </summary>
        [MemoryPackOrder(5)]
        public DateTime LastAttackTime { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        [MemoryPackOrder(6)]
        public DTO_EnemyLocation Location { get; set; }

        /// <summary>
        /// 드랍 아이템.
        /// </summary>
        [MemoryPackOrder(7)]
        public List<Type_Item> DropItems { get; set; }
    }
}
