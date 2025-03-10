using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.WebAPI.Session
{
    /// <summary>
    /// GameSession.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GameSession
    {
        /// <summary>
        /// 유저 Id.
        /// </summary>
        [MemoryPackOrder(0)]
        public string UserId { get; init; }

        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Uid { get; init; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(2)]
        public string Pid { get; init; }

        /// <summary>
        /// IP Address.
        /// </summary>
        [MemoryPackOrder(3)]
        public string Address { get; init; }

        /// <summary>
        /// Country
        /// </summary>
        [MemoryPackOrder(4)]
        public string Country { get; init; }
    }
}
