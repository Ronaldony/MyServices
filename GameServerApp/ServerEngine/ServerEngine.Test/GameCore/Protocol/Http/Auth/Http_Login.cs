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
        public string Id { get; set; }
        
        [MemoryPackOrder(1)]
        public byte[] Pass { get; set; }
    }

    /// <summary>
    /// RES_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class RES_Login : ResponseBase
    {
        [MemoryPackOrder(1)]
        public DTO_PlayerInfo PlayerInfo { get; set; }
    }
}
