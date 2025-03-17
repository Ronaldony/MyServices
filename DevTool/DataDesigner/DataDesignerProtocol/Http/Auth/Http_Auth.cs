using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http.Auth
{
    /// <summary>
    /// REQ_Auth.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class REQ_Auth
    {
        /// <summary>
        /// user id.
        /// </summary>
        [MemoryPackOrder(0)]
        public string UserId { get; set; }
    }

    /// <summary>
    /// REQ_Auth.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_Auth : ResponseBase
    {
        /// <summary>
        /// AccessToken.
        /// </summary>
        [MemoryPackOrder(1)]
        public string AccessToken { get; set; }

        /// <summary>
        /// RefreshToken.
        /// </summary>
        [MemoryPackOrder(2)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// RefrshTokenExpire.
        /// </summary>
        [MemoryPackOrder(3)]
        public DateTime RefrshTokenExpire { get; set; }

        /// <summary>
        /// PlayerDatas.
        /// key: pid.
        /// value: player name.
        /// </summary>
        [MemoryPackOrder(4)]
        public Dictionary<string, string> PlayerDataDict { get; set; }
    }
}
