using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Inventory
{
    /// <summary>
    /// 
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Equipment
    {
        public DTO_Equipment()
        {

        }

        /// <summary>
        /// TB_Weapon id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int Weapon { get; set; }

        /// <summary>
        /// Top.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Top { get; set; }
    }
}
