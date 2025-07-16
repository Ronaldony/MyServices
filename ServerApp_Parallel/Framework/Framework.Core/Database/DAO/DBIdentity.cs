using Dapper;
using MySqlConnector;
using System.Data;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.DBO;

    /// <summary>
    /// DBIdentity.
    /// </summary>
    public class DBIdentity
    {
        private readonly string _connectString;

        public DBIdentity()
        {
            _connectString = GameConfigManager.GetMySQLConnectString("Identity");
        }

        /// <summary>
        /// CreateIdentityAsync.
        /// </summary>
        public async Task<bool> CreateIdentityAsync(string uid, string token, string refreshToken, DateTime tokenExpireTime)
        {
            const string SQL = """
                                INSERT INTO TBL_Identity 
                                    (uid, token, refresh_token, token_expire, regtime) 
                                    VALUES (@Uid, @Token, @RefreshToken, @TokenExpire, NOW())
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                        Token = token,
                        RefreshToken = refreshToken,
                        TokenExpire = tokenExpireTime,
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
        /// GetIdentity.
        /// </summary>
        public async Task<DBO_Identity> SelectOrDefaultAsync(string uid)
        {
            const string SQL = """
                                SELECT * FROM TBL_Identity 
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryFirstOrDefaultAsync<DBO_Identity>(
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
        /// GetIdentity.
        /// </summary>
        public DBO_Identity SelectOrDefault(string uid)
        {
            const string SQL = """
                                SELECT * FROM TBL_Identity 
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                return connection.QueryFirstOrDefault<DBO_Identity>(
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
        /// UpdateIdentity_AccessTokenAsync.
        /// </summary>
        public async Task<bool> UpdatePidAsync(string uid, string pid)
        {
            const string SQL = """
                                UPDATE TBL_Identity SET
                                    pid=@Pid
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                        Pid = pid
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
        /// UpdateIdentity_AccessTokenAsync.
        /// </summary>
        public async Task<bool> UpdateTokenAsync(string uid, string token, string refreshToken, DateTime tokenExpireTime)
        {
            const string SQL = """
                                UPDATE TBL_Identity SET 
                                    token=@Token, refresh_token=@RefreshToken, token_expire=@TokenExpire 
                                    WHERE uid=@Uid
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        Uid = uid,
                        Token = token,
                        refreshToken = refreshToken,
                        TokenExpire = tokenExpireTime
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
