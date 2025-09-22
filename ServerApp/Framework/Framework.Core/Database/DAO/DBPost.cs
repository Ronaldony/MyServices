
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;
using System.Text;

namespace Framework.Core.Database.DAO
{
    using Akka.Dispatch.SysMsg;
    using Framework.Config.ConfigManager;
    using Framework.Core.Database.DBO;
    using Framework.Core.Types;

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
        public async Task<bool> Create(string postUid, string pid, byte[] items, string text)
        {
            const string SQL = """
                                INSERT INTO TBL_Post 
                                    (post_uid, pid, step, items, text, regtime) 
                                    VALUES (@PostUid, @Pid, @Step, @Items, @Text, NOW())
                                """;

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        PostUid = postUid,
                        Pid = pid,
                        Items = items,
                        Step = 0,
                        Text = text
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
            var sb = new StringBuilder("INSERT INTO TBL_Post (post_uid, pid, step, items, text, regtime) VALUES ");

            // Last.
            var lastPost = posts.Last();

            using (var connection = new MySqlConnection(_connectString))
            {
                // connection open.
                connection.Open();

                // Command.
                using (var command = new MySqlCommand())
                {
                    int idx = 0;

                    foreach (var post in posts)
                    {
                        // Add parameters.
                        command.Parameters.AddWithValue("@PostUid" + idx, post.Uid);
                        command.Parameters.AddWithValue("@Pid" + idx, post.Pid);
                        command.Parameters.AddWithValue("@Step" + idx, post.PostStep);
                        command.Parameters.AddWithValue("@Items" + idx, post.Items);
                        command.Parameters.AddWithValue("@Text" + idx, post.Text);

                        // Query.
                        sb.AppendFormat("(@PostUid{0}, @Pid{0}, @Step{0}, @Items{0}, @Text{0}, NOW())", idx);

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
        }

        /// <summary>
        /// Select.
        /// </summary>
        public async Task<IEnumerable<DBO_Post>> SelectPostsAsync(Type_PostStep step)
        {
            const string SQL = "SELECT * FROM TBL_Post WHERE step=@Step LIMIT 30";

            using (var connection = new MySqlConnection(_connectString))
            {
                return await connection.QueryAsync<DBO_Post>(
                    sql: SQL,
                    param: new
                    {
                        Step = step
                    },
                    commandTimeout: 10,
                    commandType: CommandType.Text
                    );
            }
        }

        /// <summary>
        /// Update post status.
        /// </summary>
        public async Task<bool> UpdatePostStatusAsync(IEnumerable<string> postUids, Type_PostStep step)
        {
            const string SQL = "UPDATE TBL_Post SET step=@Step WHERE post_uid IN @PostUids";

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        Step = step,
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

        /// <summary>
        /// Delete post.
        /// </summary>
        public async Task<bool> DeleteAsync(string postUid)
        {
            const string SQL = "DELETE FROM TBL_Post WHERE post_uid = @PostUid";

            using (var connection = new MySqlConnection(_connectString))
            {
                var rowAffect = await connection.ExecuteAsync(
                    sql: SQL,
                    param: new
                    {
                        PostUid = postUid
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
