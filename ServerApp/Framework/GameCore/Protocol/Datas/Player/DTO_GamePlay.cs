using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Play
{
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Player.Inventory;

    /// <summary>
    /// DTO_GamePlay.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_GamePlay
    {
        public DTO_GamePlay()
        {
            Equipment = new DTO_Equipment();
            InventoryInfo = new DTO_InventoryInfo();
        }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PlayerName { get; set; }
        
        /// <summary>
        /// Update time for DTO_GamePlay.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// Pos.
        /// </summary>
        [MemoryPackOrder(3)]
        public Vector2 Pos { get; set; }

        /// <summary>
        /// Health.
        /// </summary>
        [MemoryPackOrder(4)]
        public int Health { get; set; }

        /// <summary>
        /// Velocity.
        /// </summary>
        [MemoryPackOrder(5)]
        public double Velocity { get; set; }

        /// <summary>
        /// Equipment.
        /// </summary>
        [MemoryPackOrder(6)]
        public DTO_Equipment Equipment { get; set; }

        /// <summary>
        /// InventoryInfo.
        /// </summary>
        [MemoryPackOrder(7)]
        public DTO_InventoryInfo InventoryInfo { get; set; }
    }
}
