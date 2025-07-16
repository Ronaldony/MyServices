
using MemoryPack;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Enemy
{
    /// <summary>
    /// DTO_EnemyPosition.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_EnemyLocation
    {
        public DTO_EnemyLocation()
        {
        }

        /// <summary>
        /// 체력.
        /// </summary>
        [MemoryPackOrder(0)]
        public int Veltocity { get; set; }

        /// <summary>
        /// 스폰 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 SpawnPos { get; set; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// 다음 이동 위치.
        /// </summary>
        [MemoryPackOrder(3)]
        public Vector2 NextPos { get; set; }

        /// <summary>
        /// 다음 이동 위치 방향.
        /// </summary>
        [MemoryPackOrder(4)]
        public Vector2 NextPosDir { get; set; }

        /// <summary>
        /// 이동 패턴.
        /// </summary>
        [MemoryPackOrder(6)]
        public Type_MovePattern MovePattern { get; set; }
    }
}
