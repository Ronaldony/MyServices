using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Status
{
    /// <summary>
    /// 위치 정보
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Position
    {
        /// <summary>
        /// Area.
        /// </summary>s
        [MemoryPackOrder(0)]
        public Type_Area Area { get; set; }

        /// <summary>
        /// Pos.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// Pos.
        /// </summary>
        [MemoryPackOrder(2)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
