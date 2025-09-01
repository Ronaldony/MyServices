using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player
{
    using GameCore.Datas.Post;

    /// <summary>
    /// 우편함 정보.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_PostInfo : DTO_InfoBase
    {
        public DTO_PostInfo()
        {
            Posts = new Dictionary<string, DTO_Post>();
        }

        /// <summary>
        /// 우편물.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<string, DTO_Post> Posts { get; set; }

        /// <summary>
        /// 생성 시간.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime RegTime { get; set; }
    }
}
