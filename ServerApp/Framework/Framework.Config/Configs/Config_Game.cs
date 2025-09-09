using ServerEngine.Config.Database;
using ServerEngine.Config.MessageBus;

namespace Framework.Config.Configs
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
        /// Frame rate.
        /// </summary>
        public int FrameRate { get; set; }

        /// <summary>
        /// ConfigCouchbase.
        /// </summary>
        public Config_Couchbase Couchbase { get; set; }

        /// <summary>
        /// Config_MessageBus.
        /// </summary>
        public Config_MessageBus MessageBus { get; set; }
    }
}
