
namespace ServerEngine.Config.ConfigManager
{
    using ServerEngine.Config;

    /// <summary>
    /// Game configuration manager.
    /// </summary>
    public static class GameConfigManager
    {
        /// <summary>
        /// Game configuration.
        /// </summary>
        public static Config_Game _consulGame { get; private set; }

        /// <summary>
        /// DB connections.
        /// key: DB alias.
        /// value: DB connection string.
        /// </summary>
        private static Dictionary<string, string> _connectStringDic;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static bool Initialize(Config_Game consulGame, List<Config_Database> consulDatabase)
        {
            // ConsuleGame.
            _consulGame = consulGame;

            // Database.
            _connectStringDic = new Dictionary<string, string>();

            foreach (var databse in consulDatabase)
            {
                _connectStringDic.Add(databse.Alias, databse.ConnectString);
            }

            return false;
        }

        /// <summary>
        /// Get connection string of alias.
        /// </summary>
        public static string GetDBConnectString(string alias)
        {
            if (true == _connectStringDic.ContainsKey(alias))
            {
                // exist.
                return _connectStringDic[alias];
            }

            return string.Empty;
        }
    }
}
