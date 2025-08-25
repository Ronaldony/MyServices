
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;
using System.Text;

namespace Framework.Core.Database.DAO
{
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.DBO;

    /// <summary>
    /// DBPost.
    /// </summary>
    public class DBPost
    {
        private readonly ILogger<DBPost> _logger;       
        private readonly string _connectString;

        public DBPost(ILogger<DBPost> logger)
        {
            _logger = logger;
            _connectString = GameConfigManager.GetMySQLConnectString("Game");
        }

        /// <summary>
        /// Create.
        /// </summary>
        public async Task<bool> Create(string postUid, string pid, byte[] items)
        {
            const string SQL = """
                                INSERT INTO TBL_Post 
                                    (post_uid, pid, items, regtime) 
                                    VALUES (@PostUid, @Pid, @Items, NOW())
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        PostUid = postUid,
                        Pid = pid,
                        Items = items
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
        /// Create.
        /// </summary>
        public bool CreateBluk(List<DBO_Post> posts)
        {
            if (false == posts.Any())
            {
                return true;
            }

            // SQL.
            var sb = new StringBuilder("INSERT INTO TBL_Post (post_uid, pid, items, regtime) VALUES ");

            // Last.
            var lastPost = posts.Last();

            using (var connection = new MySqlConnection(_connectString))
            {
                // connection open.
                connection.Open();

                try
                {
                    // Command.
                    using (var command = new MySqlCommand())
                    {
                        int idx = 0;

                        foreach (var post in posts)
                        {
                            // Add parameters.
                            command.Parameters.AddWithValue("@PostUid" + idx, post.Uid);
                            command.Parameters.AddWithValue("@Pid" + idx, post.Pid);
                            command.Parameters.AddWithValue("@Items" + idx, post.Items);

                            // Query.
                            sb.AppendFormat("(@PostUid{0}, @Pid{0}, @Items{0}, NOW())", idx);

                            if (lastPost == post)
                            {
                                // Last query.
                                sb.Append(";");
                            }
                            else
                            {
                                sb.Append(", ");
                            }

                            ++idx;
                        }

                        // Add Last 

                        command.Connection = connection;
                        command.CommandText = sb.ToString();

                        var rowAffected = command.ExecuteNonQuery();

                        if (rowAffected > 0)
                        {
                            return true;
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Select .
        /// </summary>
        public async Task<IEnumerable<DBO_Post>> SelectLimitAsync()
        {
            const string SQL = "SELECT * FROM TBL_Post LIMIT 30";

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryAsync<DBO_Post>(
                    sql: SQL,
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }

        /// <summary>
        /// Delete posts.
        /// </summary>
        public async Task<bool> DeletesAsync(List<string> postUids)
        {
            const string SQL = "DELETE FROM TBL_Post WHERE post_uid in @PostUids";

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        PostUids = postUids
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );

                if (rowAffect > 0)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
