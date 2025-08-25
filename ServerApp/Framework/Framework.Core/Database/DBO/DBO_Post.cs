using Dapper.ColumnMapper;

namespace Framework.Core.Database.DBO
{
    using Framework.Core.Database.Attributes;

    /// <summary>
    /// DBO_Post.
    /// </summary>
    [DapperMapping]
    public class DBO_Post
    {
        /// <summary>
        /// 우편 Uid.
        /// </summary>
        [ColumnMapping("post_uid")]
        public string Uid { get; set; }

        /// <summary>
        /// 지급 Pid.
        /// </summary>
        [ColumnMapping("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// Post items.
        /// DTO_Item list.
        /// </summary>
        [ColumnMapping("items")]
        public byte[] Items { get; set; }

        /// <summary>
        /// RegTime.
        /// </summary>
        [ColumnMapping("regtime")]
        public DateTime RegTime { get; set; }
    }
}
