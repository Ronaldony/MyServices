using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;

namespace Framework.Core.Database.DAO
{
    using Framework.Core.Database.DBO;
    using ServerEngine.Config.ConfigManager;

    public class DBUser
    {
        private readonly ILogger<DBUser> _logger;

        private readonly string _connectString;

        public DBUser(ILogger<DBUser> logger)
        {
            _logger = logger;

            _connectString = GameConfigManager.GetMySQLConnectString("Identity");
        }

        /// <summary>
        /// GetByUserIdAsync.
        /// </summary>
        public async Task<DBO_User> SelectUserOrDefualtAsync(string userId)
        {
            const string SQL = "SELECT * FROM TBL_User WHERE user_id=@UserId";

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryFirstOrDefaultAsync<DBO_User>(
                    sql: SQL,
                    param: new
                    {
                        UserId = userId,
                    },
                    commandTimeout: 15,
                    commandType: CommandType.Text);
            }
        }

        public bool UpdateUid(string userId, string uid)
        {
            return default;
        }
	}
}
