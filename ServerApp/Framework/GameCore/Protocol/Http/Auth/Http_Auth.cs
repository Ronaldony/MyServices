using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http.Auth
{
    using GameCore.Datas.Player;

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
    /// RES_Auth.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_Auth : ResponseBase
    {
        [MemoryPackOrder(1)]
        public DTO_PlayerInfo PlayerInfo { get; set; }
    }
}
