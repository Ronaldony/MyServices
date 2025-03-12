
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers
{
	using Framework.Core.Database.DAO;
	using Framework.Core.Services;
	using Framework.Core.WebAPI.ActionFilter;
	using Framework.WebAPI;
	using Framework.WebAPI.Attributes;
	using Framework.WebAPI.Controllers;
	using GameCore.Datas.Player;
	using GameCore.Http.Auth;
	using ServerEngine.Core.Services.Interfaces;
	using ServerEngine.Core.Util;

	/// <summary>
	/// 인증 처리 컨트롤러.
	/// </summary>
	public class AuthController : GameController
	{
		private readonly ILogger<AuthController> _logger;
		private readonly DBIdentity _dbIdentity;
		private readonly DBUser _dbUser;
		private readonly IUniqueIdService _uniqueIdService;
		private readonly GameSessionService _gameSessionService;

		public AuthController(
			ILogger<AuthController> logger,
			IServiceProvider serviceProvider) 
			: base(logger)
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
		[IgnoreAuth]
		[Route("auth/auth_local")]
		[ServiceFilter(typeof(AuthActionFilter))]
		public async Task<RES_Auth> ProcessAsync([FromBody] REQ_Auth req)
		{
			if (true == string.IsNullOrEmpty(req.UserId))
			{
				throw new ServerException(2, $"local user id is empty.");
			}

			// database userlocal data get
			var dboIdentityUserLocal = _dbUser.GetByUserId(req.UserId);
			if (null == dboIdentityUserLocal)
			{
				throw new ServerException(1, $"local user not exist. user_id: {req.UserId}");
			}

			bool isNewUser = false;

			// Uid가 없는경우 신규 가입자로
			if (true == string.IsNullOrEmpty(dboIdentityUserLocal.Uid))
			{
				isNewUser = true;
			}

			var nowTime = TimeUtil.Now;

			var uid = dboIdentityUserLocal.Uid;
			if (true == string.IsNullOrEmpty(uid))
			{
				uid = _uniqueIdService.GenerateId();
			}

			//var accessToken = new AccessToken
			//{
			//	UserId = dboIdentityUserLocal.UserId,
			//	IdentityType = Type_Identity.Local,
			//	Uid = uid,
			//	SignedDate = nowTime,
			//	SignedUnixTime = TimeUtil.DateTimeToUnixTime(nowTime),
			//	ExpireTime = nowTime.AddDays(ServerConfigurationManager.TokenExpireAddDate),
			//};

			//var accessTokenString = _gameSessionService.MakeTokenString(accessToken);
			//var refreshToken = _gameSessionService.GenerateRefreshToken();
			//var refreshTokenExpiry = nowTime.AddDays(ServerConfigurationManager.RefreshTokenExpiryAddDay);

			// 신규유저인 경우
			if (true == isNewUser)
			{
				//if (false == await _dbIdentity.CreateIdentityAsync(accessToken.Uid, accessTokenString, refreshToken, refreshTokenExpiry))
				{
					throw new ServerException(3, "identity insert error.");
				}

				//if (false == _dbUser.UpdateUid(dboIdentityUserLocal.UserId, accessToken.Uid))
				//{
				//	throw new ServerException(2, $"user uid update error. {dboIdentityUserLocal.UserId}, {accessToken.Uid}");
				//}
			}
			else
			{
				var dboIdentity = _dbIdentity.GetIdentity(dboIdentityUserLocal.Uid);

				// 기존가입자이지만 Uid는 존재하고 Identity가 없는경우 추가로 넣어준다.
				// TBL_UserLocal에는 Uid가 설정되어 있고 TBL_Identity에는 없는 경우
				if (null == dboIdentity)
				{
					//if (false == _dbUser.UpdateUid(dboIdentityUserLocal.UserId, accessToken.Uid))
					{
						throw new ServerException(2, "identity userlocal update error.");
					}

					//if (false == await _dbIdentity.CreateIdentityAsync(accessToken.Uid, accessTokenString, refreshToken, refreshTokenExpiry))
					{
						throw new ServerException(3, "identity insert error.");
					}
				}
				else
				{
					//if (false == await _dbIdentity.UpdateIdentity_AccessTokenAsync(accessToken.Uid, accessTokenString))
					{
						//throw new ServerException(2, $"identity update accesstoken error. {accessToken.Uid}");
					}
				}
			}

			//_logger.LogDebug($"[{IPAddress}] [HttpREQ_AuthLocal] [uid:{uid}/UserId:{accessToken.UserId}]");

			return new RES_Auth
			{
				PlayerInfo = new DTO_PlayerInfo()
			};
		}
	}
}
