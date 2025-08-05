
namespace Framework.Config.ConfigManager
{
    using ServerEngine.Config.Database;

    /// <summary>
    /// Game configuration manager.
    /// </summary>
    public static class GameConfigManager
    {
        /// <summary>
        /// Game configuration.
        /// </summary>
        public static Config_Game ConfigGame { get; private set; }

        /// <summary>
        /// MySQL connections.
        /// key: MySQL alias.
        /// value: MySQL connection string.
        /// </summary>
        private static Dictionary<string, string> _mysqlConnectStringDict;

        /// <summary>
        /// Couchbase connections.
        /// key: Couchbase alias.
        /// value: Couchbase connection string.
        /// </summary>
        private static Dictionary<string, Config_CouchbaseConnection> _couchbaseConnectionDict;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static bool Initialize(
            Config_Game consulGame, 
            List<Config_MySQLConnection> mysqlConnections,
            List<Config_CouchbaseConnection> couchbaseConnections)
        {
            // ConsuleGame.
            ConfigGame = consulGame;

            // MySQL.
            _mysqlConnectStringDict = new Dictionary<string, string>();

            foreach (var connection in mysqlConnections)
            {
                _mysqlConnectStringDict.Add(connection.Alias, connection.ConnectString);
            }

            // Couchbase.
            _couchbaseConnectionDict = new Dictionary<string, Config_CouchbaseConnection>();

            foreach (var connection in couchbaseConnections)
            {
                _couchbaseConnectionDict.Add(connection.Alias, connection);
            }

            return false;
        }

        /// <summary>
        /// Get MySQL connection.
        /// </summary>
        public static string GetMySQLConnectString(string alias)
        {
            if (true == _mysqlConnectStringDict.ContainsKey(alias))
            {
                // exist.
                return _mysqlConnectStringDict[alias];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get couchbase connection.
        /// </summary>
        public static Config_CouchbaseConnection GetCouchbaseConnection(string alias)
        {
            if (true == _couchbaseConnectionDict.ContainsKey(alias))
            {
                // exist.
                return _couchbaseConnectionDict[alias];
            }

            return null;
        }
    }
}
