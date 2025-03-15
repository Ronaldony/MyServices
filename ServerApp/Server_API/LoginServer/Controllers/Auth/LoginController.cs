
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Framework.Core.Services;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Http.Auth;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;

    /// <summary>
    /// 인증 처리 컨트롤러.
    /// </summary>
    public class LoginController : GameController
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IUniqueIdService _uniqueIdService;
        private readonly GameSessionService _gameSessionService;

        private readonly DBIdentity _dbIdentity;
        private readonly DBUser _dbUser;

        public LoginController(
            ILogger<LoginController> logger,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logger = logger;

            // Serviecs.
            _gameSessionService = serviceProvider.GetRequiredService<GameSessionService>();
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();

            // DAOs.
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
            _dbUser = serviceProvider.GetRequiredService<DBUser>();
        }

        [HttpPost]
        [Route("auth/login")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_Login> ProcessAsync([FromBody] REQ_Login req)
        {
            if (true == string.IsNullOrEmpty(req.UserId))
            {
                throw new ServerException(GameErrorCode.Login_EmptyUserId, $"local user id is empty.");
            }

            bool isNewUser = false;

            // database userlocal data get
            var dboUser = await _dbUser.SelectUserOrDefualtAsync(req.UserId);
            var uid = dboUser?.Uid;

            if (true == string.IsNullOrEmpty(uid))
            {
                // Uid가 없는경우 신규 가입자.
                uid = _uniqueIdService.GenerateId();
                isNewUser = true;
            }

            var nowTime = TimeUtil.Now;
            var accessToken = _uniqueIdService.GenerateId();
            var refreshToken = _uniqueIdService.GenerateId();
            var refreshTokenExpire = nowTime.AddDays(1);

            // 신규유저인 경우
            if (true == isNewUser)
            {
                if (false == await _dbIdentity.CreateIdentityAsync(uid, accessToken, refreshToken, refreshTokenExpire))
                {
                    throw new ServerException(GameErrorCode.DBError, "identity insert error.");
                }

                if (false == await _dbUser.UpdateUidAsync(dboUser.UserId, uid))
                {
                    throw new ServerException(GameErrorCode.DBError, $"user uid update error. {dboUser.UserId}, {uid}");
                }
            }
            else
            {
                var dboIdentity = _dbIdentity.GetIdentity(dboUser.Uid);

                // 기존가입자이지만 Uid는 존재하고 Identity가 없는경우 추가로 넣어준다.
                // TBL_UserLocal에는 Uid가 설정되어 있고 TBL_Identity에는 없는 경우
                if (null == dboIdentity)
                {
                    if (false == await _dbUser.UpdateUidAsync(dboUser.UserId, uid))
                    {
                        throw new ServerException(GameErrorCode.DBError, "identity userlocal update error.");
                    }

                    if (false == await _dbIdentity.CreateIdentityAsync(uid, accessToken, refreshToken, refreshTokenExpire))
                    {
                        throw new ServerException(GameErrorCode.DBError, "identity insert error.");
                    }
                }
                else
                {
                    if (false == await _dbIdentity.UpdateAccessTokenAsync(uid, accessToken))
                    {
                        throw new ServerException(GameErrorCode.DBError, $"identity update accesstoken error. {uid}");
                    }
                }
            }

            // Session info.
            var sessionInfo = new SessionInfo
            {
                Address = this.Address,
                UserId = req.UserId,
                Uid = uid,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpireTokenTIme = refreshTokenExpire,
            };

            var sessionInfoObject = GetObject<SessionInfoObject>();

            if (false == await sessionInfoObject.UpsertAsync(uid, sessionInfo))
            {
                throw new ServerException(GameErrorCode.DBError, $"upsert session info error. {uid}");
            }

            return new RES_Login
            {
                Uid = uid,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
    }
}
