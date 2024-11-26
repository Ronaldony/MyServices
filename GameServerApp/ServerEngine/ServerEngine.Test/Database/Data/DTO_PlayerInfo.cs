
using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Database.Data
{
    using ServerEngine.Core.DataObject;

    /// <summary>
    /// DTO_PlayerInfo.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerInfo : DataObjectBase
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }
        
        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Uid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [MemoryPackOrder(2)]
        public string PlayerName { get; set; }

        /// <summary>
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(3)]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public override void TryReset()
        {
            Pid = string.Empty;
            PlayerName = string.Empty;
            RegTime = DateTime.MinValue;
        }
    }
}
