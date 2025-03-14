using ServerEngine.Config.Database;

namespace ServerEngine.Config
{
    /// <summary>
    /// Configuration for Game server.
    /// </summary>
    public class Config_Game
    {
        /// <summary>
        /// Environment for server.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Service language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// SnowflakeBaseTime.
        /// </summary>
        public DateTime SnowflakeBaseTime { get; set; }

        /// <summary>
        /// ConfigCouchbase.
        /// </summary>
        public Config_Couchbase Couchbase { get; set; }
    }
}
