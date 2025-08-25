using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Post
{
    using GameCore.Datas.Items;

    /// <summary>
    /// DTO_Post.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Post
    {
        public DTO_Post()
        {
            Items = new List<DTO_Item>();
        }

        /// <summary>
        /// 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string PostUid { get; set; }

        /// <summary>
        /// 우편물.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_Item> Items { get; set; }
    }
}
