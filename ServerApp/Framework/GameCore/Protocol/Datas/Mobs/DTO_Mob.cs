using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Mobs
{
    using GameCore.Datas.Plays;

    /// <summary>
    /// DTO_Mob.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Mob
    {
        public DTO_Mob()
        {
            Stat = new DTO_Stat();
            Location = new DTO_Location();
            DropItems = new List<Type_Item>();
        }

        /// <summary>
        /// 몹 id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int MobId { get; set; }

        /// <summary>
        /// 공격 유형.
        /// </summary>
        [MemoryPackOrder(2)]
        public Type_MobAttack AttackType { get; set; }

        /// <summary>
        /// 능력치.
        /// </summary>
        [MemoryPackOrder(3)]
        public DTO_Stat Stat { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        [MemoryPackOrder(4)]
        public DTO_Location Location { get; set; }

        /// <summary>
        /// 드랍 아이템.
        /// </summary>
        [MemoryPackOrder(5)]
        public List<Type_Item> DropItems { get; set; }
    }
}
