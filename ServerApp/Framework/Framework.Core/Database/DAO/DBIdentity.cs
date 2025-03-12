using Framework.Core.Database.DBO;
using Microsoft.Extensions.Logging;

namespace Framework.Core.Database.DAO
{
    public class DBIdentity
    {
        private ILogger<DBIdentity> _logger;

        public DBIdentity(ILogger<DBIdentity> logger)
        {
            _logger = logger;
		}

		/// <summary>
		/// CreateIdentityAsync.
		/// </summary>
		public async Task<bool> CreateIdentityAsync(string uid, string token, string refreshToken, DateTime tokenExpireTime)
        {

            await Task.Delay(2);

			return default;
        }

		/// <summary>
		/// GetIdentity.
		/// </summary>
		public DBO_Identity GetIdentity(string uid)
        {
            return default;
		}

		/// <summary>
		/// UpdateIdentity_AccessTokenAsync.
		/// </summary>
		public async Task<bool> UpdateIdentity_AccessTokenAsync(string uid, string token)
        {

            await Task.Delay(1);

            return default;
        }
	}
}
