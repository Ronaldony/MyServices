
namespace Framework.Config.ConfigManager
{
    using Framework.Config.Configs;
    using Framework.Config.MessageBus;
    using ServerEngine.Config.Database;
    using ServerEngine.Config.MessageBus;

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
        /// Game configuration.
        /// </summary>
        public static Config_MessageBus ConfigMessageBus { get; private set; }

        /// <summary>
        /// MySQL connections.
        /// key: MySQL alias.
        /// value: MySQL connection string.
        /// </summary>
        private static Dictionary<string, string> _mysqlConnectStringDict;

        /// <summary>
        /// MessageBus.
        /// key: MessageBus name.
        /// value: .
        /// </summary>
        private static Dictionary<string, Config_MessageBusData> _messageBusDataDict;

        /// <summary>
        /// Couchbase connections.
        /// key: Couchbase alias.
        /// value: Couchbase connection string.
        /// </summary>
        private static Dictionary<string, Config_CouchbaseConnection> _couchbaseConnectionDict;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static void Initialize(
            Config_Game configGame, 
            List<Config_MySQLConnection> mysqlConnections,
            List<Config_CouchbaseConnection> couchbaseConnections,
            List<Config_MessageBusData> messageBusDatas)
        {
            // ConsuleGame.
            ConfigGame = configGame;

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

            // Messagebus.
            _messageBusDataDict = new Dictionary<string, Config_MessageBusData>();

            foreach (var data in messageBusDatas)
            {
                _messageBusDataDict.Add(data.Alias, data);
            }
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

        /// <summary>
        /// Get messagebus data.
        /// </summary>
        public static Config_MessageBusData GetMessageBusData(string alias)
        {
            if (true == _messageBusDataDict.ContainsKey(alias))
            {
                // exist.
                return _messageBusDataDict[alias];
            }

            return null;
        }
    }
}
