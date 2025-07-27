using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace GameCore.Datas.Enemy
{
    using GameCore.Datas.Items;
    using GameCore.Datas.Plays;

    /// <summary>
    /// DTO_Enemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Enemy
    {
        public DTO_Enemy()
        {
            Stat = new DTO_Stat();
            Location = new DTO_Location();
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
        /// 행동 유형.
        /// </summary>
        [MemoryPackOrder(2)]
        public Type_AIState AIState { get; set; }

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
