
using MemoryPack;
using ServerEngine.Database.Data;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Database.Data
{
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PlayerInfo : DataObjectBase
    {
        public DTO_PlayerInfo()
        { 
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
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime RegTime { get; set; }
    }
}
