using Framework.Core.Database.DBO;
using Microsoft.Extensions.Logging;

namespace Framework.Core.Database.DAO
{
    public class DBUser
    {
        private readonly ILogger<DBUser> _logger;

        public DBUser(ILogger<DBUser> logger)
        {
            _logger = logger;
		}

        public DBO_User GetByUserId(string userId)
        {
            return default;
        }

        public bool UpdateUid(string userId, string uid)
        {
            return default;
        }
	}
}
