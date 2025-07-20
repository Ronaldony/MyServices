using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Plays
{
    /// <summary>
    /// 위치 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Location
    {
        /// <summary>
        /// 현재 지역.
        /// </summary>s
        [MemoryPackOrder(0)]
        public Type_Area Area { get; set; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// 종료 위치 지점.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 PositionEnd { get; set; }

        /// <summary>
        /// 이동 방향.
        /// </summary>
        [MemoryPackOrder(3)]
        public Vector2 MoveDirection { get; set; }
    }
}
