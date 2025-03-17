using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http.Auth
{
    using GameCore.Datas.Player;

    /// <summary>
    /// REQ_Login.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class REQ_Login
    {
        /// <summary>
        /// user id.
        /// </summary>
        [MemoryPackOrder(0)]
        public string UserId { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(2)]
        public string Pid { get; set; }

        /// <summary>
        /// AccessToken.
        /// </summary>
        [MemoryPackOrder(3)]
        public string AccessToken { get; set; }
    }

    /// <summary>
    /// RES_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_Login : ResponseBase
    {
        /// <summary>
        /// PlayerInfo.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_PlayerInfo PlayerInfo { get; set; }
    }
}
