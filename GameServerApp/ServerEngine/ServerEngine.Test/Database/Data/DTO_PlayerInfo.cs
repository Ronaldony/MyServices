
using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Database.Data
{
    using ServerEngine.Core.DataObject;

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerInfo : DataObjectBase
    {
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
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public override bool TryReset()
        {
            Pid = string.Empty;
            PlayerName = string.Empty;
            RegTime = DateTime.MinValue;

            return true;
        }
    }
}
