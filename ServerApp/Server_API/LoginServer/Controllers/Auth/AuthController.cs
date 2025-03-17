
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Http.Auth;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;

    /// <summary>
    /// 인증 컨트롤러.
    /// 클라이언트 인증 처리.
    /// </summary>
    public class AuthController : GameController
    {
        private readonly IUniqueIdService _uniqueIdService;

        private readonly DBIdentity _dbIdentity;
        private readonly DBUser _dbUser;
        private readonly DBPlayerData _dbPlayerData;

        public AuthController(
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            // Serviecs.
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();

            // DB.
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
            _dbUser = serviceProvider.GetRequiredService<DBUser>();
            _dbPlayerData = serviceProvider.GetRequiredService<DBPlayerData>();
        }

        [HttpPost]
        [Route("auth/auth")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_Auth> ProcessAsync([FromBody] REQ_Auth req)
        {
            if (true == string.IsNullOrEmpty(req.UserId))
            {
                throw new ServerException(GameErrorCode.Auth_EmptyUserId, $"local user id is empty.");
            }

            // 실제 서비스에서는 현 시점에서 SAML, SSO 등의 인증 솔루션을 통해 인증 정보(UserId 등) 유효성 검사.

            var nowTime = TimeUtil.Now;
            bool isNewUser = false;

            // database userlocal data get
            var dboUser = await _dbUser.SelectUserOrDefualtAsync(req.UserId);
            var uid = dboUser?.Uid;

            if (true == string.IsNullOrEmpty(uid))
            {
                // Uid가 없는경우 신규 가입자.
                isNewUser = true;
                uid = _uniqueIdService.GenerateId();
            }

            var res = new RES_Auth();

            // 액세스 토큰 및 리프레시 토큰 발행.
            var accessToken = _uniqueIdService.GenerateId();
            var refreshToken = _uniqueIdService.GenerateId();
            var refreshTokenExpire = nowTime.AddDays(1);

            // 신규 유적와 기존 유저 구분.
            if (true == isNewUser)
            {
                // 신규유저.
                if (false == await _dbUser.CreateUserAsync(req.UserId, uid))
                {
                    throw new ServerException(GameErrorCode.DBError, $"create user error. {req.UserId}, {uid}");
                }

                if (false == await _dbIdentity.CreateIdentityAsync(uid, accessToken, refreshToken, refreshTokenExpire))
                {
                    throw new ServerException(GameErrorCode.DBError, $"create identity error. {uid}");
                }
            }
            else
            {
                var dboIdentity = _dbIdentity.SelectIdentityOrDefaultAsync(dboUser.Uid);

                // 기존가입자이지만 Uid는 존재하고 Identity가 없는경우 추가로 넣어준다.
                // TBL_User에는 Uid가 설정되어 있고 TBL_Identity에는 없는 경우
                if (null == dboIdentity)
                {
                    if (false == await _dbIdentity.CreateIdentityAsync(uid, accessToken, refreshToken, refreshTokenExpire))
                    {
                        throw new ServerException(GameErrorCode.DBError, "create identity error.");
                    }
                }
                else
                {
                    if (false == await _dbIdentity.UpdateTokenAsync(uid, accessToken, refreshToken, refreshTokenExpire))
                    {
                        throw new ServerException(GameErrorCode.DBError, $"update accesstoken error. {uid}");
                    }

                    // Set player datas.
                    var playerDatas = await _dbPlayerData.SelectPlayerDatasOrDefaultAsync(uid);

                    if (default != playerDatas)
                    {
                        res.PlayerDataDict = playerDatas.ToDictionary(d => d.Pid, d => d.PlayerName);
                    }
                }
            }

            // Response.
            res.Uid = uid;
            res.AccessToken = accessToken;
            res.RefreshToken = refreshToken;
            res.RefrshTokenExpire = refreshTokenExpire;

            return res;
        }
    }
}
