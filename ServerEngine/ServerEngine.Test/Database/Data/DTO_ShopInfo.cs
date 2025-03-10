
using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Database.Data
{
	/// <summary>
	/// DTO_ShopInfo.
	/// </summary>
	[MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_ShopInfo
    {
        /// <summary>
        /// ShopInfo RefId.
        /// </summary>
        [MemoryPackOrder(0)]
        public int RefId { get; set; }

        /// <summary>
        /// ShopInfo Schedule RefId.
        /// </summary>
        [MemoryPackOrder(1)]
        public int ScheduleRefId { get; set; }

        /// <summary>
        /// RegTime.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime RegTime { get; set; }
    }
}
