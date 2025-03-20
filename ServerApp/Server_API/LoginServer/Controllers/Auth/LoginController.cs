
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Datas.Player;
    using GameCore.Http.Auth;
    using ServerEngine.Core.Util;

    /// <summary>
    /// 로그인 컨트롤러.
    /// 인증 처리 후 전달된 Pid로 로그인 처리.
    /// </summary>
    public class LoginController : GameController
    {
        private readonly DBIdentity _dbIdentity;
        private readonly DBUser _dbUser;
        private readonly DBPlayerData _dbPlayerData;

        public LoginController(
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            // DB.
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
            _dbUser = serviceProvider.GetRequiredService<DBUser>();
            _dbPlayerData = serviceProvider.GetRequiredService<DBPlayerData>();
        }

        [HttpPost]
        [Route("auth/login")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_Login> ProcessAsync([FromBody] REQ_Login req)
        {
            var nowTime = TimeUtil.Now;

            // 유저 정보 검사.
            var dboUser = await _dbUser.SelectUserOrDefualtAsync(req.UserId);

            if (default == dboUser)
            {
                throw new ServerException(GameErrorCode.Auth_NotExistUserId, $"not exist user. {req.UserId}");
            }

            // Uid 검사.
            var uid = dboUser.Uid;

            if (false == req.Uid.Equals(uid))
            {
                throw new ServerException(GameErrorCode.Auth_NotMatchUid, $"not match uid. {req.UserId}");
            }

            // Identity 검사.
            var dboIdentity = await _dbIdentity.SelectIdentityOrDefaultAsync(uid);

            if (default == dboIdentity)
            {
                throw new ServerException(GameErrorCode.Auth_NotExistIdentity, $"not exist identity. {req.UserId}");
            }

            // Token 검사.
            if (TimeUtil.IsPastTime(nowTime, dboIdentity.TokenExpireTime))
            {
                throw new ServerException(GameErrorCode.Auth_ExpiredToken, $"expire accesstoken.");
            }

            if (false == dboIdentity.Token.Equals(req.AccessToken))
            {
                throw new ServerException(GameErrorCode.Auth_NotMatchToken, $"not match accesstoken.");
            }

            // Pid 검사.
            var dboPlayerData = await _dbPlayerData.SelectPlayerDataOrDefaultAsync(req.Pid);

            if (default == dboPlayerData)
            {
                throw new ServerException(GameErrorCode.Auth_NotExistPlayerInfo, $"not match accesstoken.");
            }

            // Save session.
            var newSessionInfo = new SessionInfo
            {
                UserId = dboUser.UserId,
                Address = this.Address,
                Uid = dboIdentity.Uid,
                RefreshToken = dboIdentity.RefreshToken,
                TokenExpireTime = dboIdentity.TokenExpireTime,
                AccessToken = dboIdentity.Token,
            };

            // SessionInfo 생성.
            var sessionInfoObject = GetObject<SessionInfoObject>();

            if (false == await sessionInfoObject.UpsertAsync(req.Pid, newSessionInfo))
            {
                throw new ServerException(GameErrorCode.Auth_NotCreatedSession, $"not match accesstoken.");
            }

            // PlayerInfo 가져오기.
            var playerInfoObejct = GetObject<PlayerInfoObject>();
            var playerInfo = await playerInfoObejct.SelectAsync<DTO_PlayerInfo>(req.Pid);

            return new RES_Login
            {
                Uid = uid,
                PlayerInfo = playerInfo,
            };
        }
    }
}
