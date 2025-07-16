using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http.Auth
{
    /// <summary>
    /// REQ_CreatePlayer.
    /// </summary>
    [HttpRouteAttribute("auth/create_player")]
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class REQ_CreatePlayer
    {
    }

    /// <summary>
    /// RES_CreatePlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_CreatePlayer : ResponseBase
    {
        /// <summary>
        /// PlayerInfo.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Pid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MemoryPackOrder(2)]
        public string PlayerName { get; set; }
    }
}
