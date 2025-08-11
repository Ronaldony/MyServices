using GameCore.Datas.Plays;
using MemoryPack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_UpdateUnitMovement.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_UpdateUnitMovement)]
    public partial class GamePacketCS_UpdateUnitMovement : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_UpdateUnitMovement.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitMovement)]
    public partial class GamePacketSC_UpdateUnitMovement : GamePacketBase_SC
    {
        public GamePacketSC_UpdateUnitMovement()
        {
            UnitLocations = new Dictionary<long, DTO_Location>();
        }

        /// <summary>
        /// Unit locations.
        /// key: unit id.
        /// value: DTO_Location.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Location> UnitLocations { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose()
        {
            UnitLocations.Clear();
        }
    }
}
