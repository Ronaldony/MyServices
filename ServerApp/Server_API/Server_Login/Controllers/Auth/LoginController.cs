
using Microsoft.AspNetCore.Mvc;

namespace Server_Login.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Http.Auth;

    /// <summary>
    /// 로그인 컨트롤러.
    /// 인증 처리 후 전달된 Pid로 로그인 처리.
    /// </summary>
    public class LoginController : GameController
    {
        private readonly ILogger<LoginController> _logger;

        private readonly DBIdentity _dbIdentity; 
        private readonly DBUser _dbUser;

        public LoginController(
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<LoginController>>();

            // DB.
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
            _dbUser = serviceProvider.GetRequiredService<DBUser>();
        }

        [HttpPost]
        [Route("auth/login")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_Login> ProcessAsync([FromBody] REQ_Login req)
        {
            // 유저 정보 검사.
            var dboUser = await _dbUser.SelectUserOrDefualtAsync(req.UserId);

            if (default == dboUser)
            {
                throw new ServerException(GameErrorCode.Login_NotExistUserId, $"not exist user. {req.UserId}");
            }

            // Identity 검사.
            var dboIdentity = await _dbIdentity.SelectOrDefaultAsync(base.Uid);

            if (default == dboIdentity)
            {
                throw new ServerException(GameErrorCode.Login_NotExistIdentity, $"not exist identity. {req.UserId}");
            }

            // SessionInfo 생성.
            var sessionInfoObject = await GetObjectAsync<SessionInfoObject>();
            var sessionInfo = sessionInfoObject.SessionInfo;

            sessionInfo.UserId = dboUser.UserId;
            sessionInfo.Address = this.Address;
            sessionInfo.Uid = dboIdentity.Uid;
            sessionInfo.RefreshToken = dboIdentity.RefreshToken;
            sessionInfo.TokenExpireTime = dboIdentity.TokenExpireTime;
            sessionInfo.AccessToken = dboIdentity.Token;

            // PlayerInfo 가져오기.
            var playerInfoObejct = await GetObjectAsync<PlayerInfoObject>();

            _logger.LogDebug($"login - uid: {dboIdentity.Uid}, pid: {base.Pid}, token: {dboIdentity.Token}");

            return new RES_Login
            {
                Uid = base.Uid,
                PlayerInfo = playerInfoObejct.PlayerInfo,
            };
        }
    }
}
