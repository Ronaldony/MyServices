using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
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
            RoamingPos = new List<Vector2>();
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
        /// 체력.
        /// </summary>
        [MemoryPackOrder(3)]
        public int MoveSpeed { get; set; }

        /// <summary>
        /// 드랍 아이템.
        /// </summary>
        [MemoryPackOrder(4)]
        public Type_Item DropItem { get; set; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        [MemoryPackOrder(5)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// 로밍 위치.
        /// </summary>
        [MemoryPackOrder(6)]
        public List<Vector2> RoamingPos { get; set; }
    }
}
