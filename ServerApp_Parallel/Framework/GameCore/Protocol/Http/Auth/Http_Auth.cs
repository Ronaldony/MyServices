using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace GameCore.Http.Auth
{
    /// <summary>
    /// REQ_Auth.
    /// </summary>
    [HttpRouteAttribute("auth/auth")]
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
        /// 신규 유저 여부.
        /// </summary>
        [MemoryPackOrder(4)]
        public bool IsNewPlayer { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(5)]
        public string Pid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [MemoryPackOrder(6)]
        public string PlayerName { get; set; }
    }
}
