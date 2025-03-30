
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Enemy
{
    /// <summary>
    /// DTO_EnemyPosition.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_EnemyPosition
    {
        public DTO_EnemyPosition()
        {
            Pos = new Vector2();
            NextPos = new Vector2();
            RoamingPos = new List<Vector2>();
        }

        /// <summary>
        /// 체력.
        /// </summary>
        [MemoryPackOrder(0)]
        public int Veltocity { get; set; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// 다음 이동 위치.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 NextPos { get; set; }

        /// <summary>
        /// 로밍 위치.
        /// </summary>
        [MemoryPackOrder(3)]
        public List<Vector2> RoamingPos { get; set; }

        /// <summary>
        /// 이동 패턴.
        /// </summary>
        [MemoryPackOrder(4)]
        public Type_MovePattern MovePattern { get; set; }

        /// <summary>
        /// 프레임 변동 시간.
        /// </summary>
        [MemoryPackOrder(5)]
        public float FrameDeltaTime { get; set; }
    }
}
