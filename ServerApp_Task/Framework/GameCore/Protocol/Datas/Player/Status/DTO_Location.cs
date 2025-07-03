using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Status
{
    /// <summary>
    /// 위치 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Location
    {
        /// <summary>
        /// Area.
        /// </summary>s
        [MemoryPackOrder(0)]
        public Type_Area Area { get; set; }

        /// <summary>
        /// Position.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// Position end.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 PositionEnd { get; set; }

        /// <summary>
        /// Pos.
        /// </summary>
        [MemoryPackOrder(3)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
