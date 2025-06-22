using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player
{
    using ServerEngine.Core.MemoryPool;

    /// <summary>
    /// DTO_PlayerInfo.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTOPlayerInfo : IDisposable
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
