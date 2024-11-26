
using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Database.Data
{
    using ServerEngine.Core.DataObject;

    /// <summary>
    /// DTO_ShopInfo.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_ShopInfo : DataObjectBase
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

        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public override void TryReset()
        {
            RefId = 0;
            ScheduleRefId = 0;
            RegTime = DateTime.MinValue;
        }
    }
}
