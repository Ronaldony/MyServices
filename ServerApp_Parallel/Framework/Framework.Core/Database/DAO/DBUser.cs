using Dapper;
using MySqlConnector;
using System.Data;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.DBO;

    public class DBUser
    {
        private readonly string _connectString;

        public DBUser()
        {
            _connectString = GameConfigManager.GetMySQLConnectString("Identity");
        }

        /// <summary>
        /// GetByUserIdAsync.
        /// </summary>
        public async Task<DBO_User> SelectUserOrDefualtAsync(string userId)
        {
            const string SQL = """
                                SELECT * FROM TBL_User 
                                    WHERE user_id=@UserId
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryFirstOrDefaultAsync<DBO_User>(
                    sql: SQL,
                    param: new
                    {
                        UserId = userId,
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }

        /// <summary>
        /// Create user.
        /// </summary>
        public async Task<bool> CreateUserAsync(string userId, string uid)
        {
            const string SQL = """
                                INSERT INTO TBL_User 
                                    (user_id, uid, regtime) 
                                    VALUES (@UserId, @Uid, NOW())
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        UserId = userId,
                        Uid = uid
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
        /// Update uid.
        /// </summary>
        public async Task<bool> UpdateUidAsync(string userId, string uid)
        {
            const string SQL = """
                                UPDATE TBL_User 
                                    SET uid=@Uid 
                                    WHERE user_id=@UserId
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        UserId = userId,
                        Uid = uid
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
    }
}
