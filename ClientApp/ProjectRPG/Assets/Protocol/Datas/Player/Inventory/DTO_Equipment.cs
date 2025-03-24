using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Inventory
{
    /// <summary>
    /// 장착 정보.
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
        /// 상의.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Top { get; set; }

        /// <summary>
        /// 신발.
        /// </summary>
        [MemoryPackOrder(2)]
        public int Shoes { get; set; }
    }
}
