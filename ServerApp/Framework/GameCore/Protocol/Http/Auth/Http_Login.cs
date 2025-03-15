using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http.Auth
{
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
    }

    /// <summary>
    /// REQ_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_Login : ResponseBase
    {
        [MemoryPackOrder(2)]
        public string AccessToken { get; set; }

        [MemoryPackOrder(3)]
        public string RefreshToken { get; set; }
    }
}
