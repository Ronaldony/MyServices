
using Dapper;
using MySqlConnector;
using System.Data;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.DBO;

    /// <summary>
    /// DBPlayerData.
    /// </summary>
    public class DBPlayerData
    {
        private readonly string _connectString;

        public DBPlayerData()
        {
            _connectString = GameConfigManager.GetMySQLConnectString("Game");
        }

        /// <summary>
        /// Create player data.
        /// </summary>
        public async Task<bool> Create(string uid, string pid, string playerName)
        {
            const string SQL = """
                                INSERT INTO TBL_PlayerData 
                                    (uid, pid, playername, regtime) 
                                    VALUES (@Uid, @Pid, @PlayerName, NOW())
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                        Pid = pid,
                        PlayerName = playerName
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );

                if (rowAffect < 1)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Select player data.
        /// </summary>
        public async Task<DBO_PlayerData> SelectOrDefaultAsync(string uid)
        {
            const string SQL = """
                                SELECT * FROM TBL_PlayerData
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryFirstOrDefaultAsync<DBO_PlayerData>(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }

        /// <summary>
        /// Select player data.
        /// </summary>
        public DBO_PlayerData SelectOrDefault(string uid)
        {
            const string SQL = """
                                SELECT * FROM TBL_PlayerData
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                return connection.QueryFirstOrDefault<DBO_PlayerData>(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }
    }
}
