using Dapper.ColumnMapper;

namespace Framework.Core.Database.DBO
{
    using Framework.Core.Database.Attributes;

    /// <summary>
    /// DBO_PlayerData.
    /// </summary>
    [DapperMapping]
    public class DBO_PlayerData
    {
        /// <summary>
        /// PId.
        /// </summary>
        [ColumnMapping("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        [ColumnMapping("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// PlayerName.
        /// </summary>
        [ColumnMapping("playername")]
        public string PlayerName { get; set; }

        /// <summary>
        /// RegTime.
        /// </summary>
        [ColumnMapping("regtime")]
        public DateTime RegTime { get; set; }
    }
}
