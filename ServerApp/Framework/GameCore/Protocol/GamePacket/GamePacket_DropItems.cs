
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Items;

    /// <summary>
    /// GamePacketCS_DropItems.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_DropItems)]
    public partial class GamePacketCS_DropItems : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_DropItems.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_DropItems)]
    public partial class GamePacketSC_DropItems : GamePacketBase_SC
    {
        public GamePacketSC_DropItems()
        {
            DropItems = new Dictionary<long, DTO_Item>();
        }

        /// <summary>
        /// Drop 위치.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// Drop items.
        /// </summary>
        [MemoryPackOrder(1)]
        public Dictionary<long, DTO_Item> DropItems { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            DropItems.Clear();
        }
    }
}
