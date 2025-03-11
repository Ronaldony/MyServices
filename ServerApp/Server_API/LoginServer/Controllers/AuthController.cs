
using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Util;
using System.Net;

namespace LoginServer.Controllers
{
	using Framework.Core.Services;
	using Framework.Core.WebAPI.ActionFilter;
	using Framework.WebAPI;
	using Framework.WebAPI.Attributes;
	using Framework.WebAPI.Controllers;

	/// <summary>
	/// 인증 처리 컨트롤러.
	/// </summary>
	public class AuthController : GameController
	{
		private readonly ILogger<AuthController> _logger;
		//private readonly DBIdentity _dbIdentity;
		//private readonly DBUserLocal _dbUserLocal;
		private readonly GameSessionService _gameSessionService;

		public AuthController(
			ILogger<AuthController> logger,
			IServiceProvider serviceProvider) 
			: base(logger)
		{
			_logger = logger;

			_gameSessionService = serviceProvider.GetRequiredService<GameSessionService>();
		}

		[HttpPost]
		[IgnoreAuth]
		[Route("auth/auth_local")]
		[ServiceFilter(typeof(AuthActionFilter))]
		public async Task<HttpRES_AuthLocal> ProcessAsync([FromBody] HttpREQ_AuthLocal req)
		{
			if (true == string.IsNullOrEmpty(req.UserId))
			{
				throw new ServerException(2, $"local user id is empty.");
			}

			// database userlocal data get
			var dboIdentityUserLocal = _dbUserLocal.GetByUserId(req.UserId);
			if (null == dboIdentityUserLocal)
			{
				throw new GameErrorException(GameErrorCode.AuthLocal_NotExist, $"local user not exist. user_id: {req.UserId}");
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
				uid = _snowflakeService.Generate();
			}

			// Maintenance check
			bool isMaintenanceCheck = false;

			var maintenanceData = _maintenanceService.GetMaintenanceData();
			if (null != maintenanceData)
			{
				if (true == TimeUtil.IsPastTime(maintenanceData.StartTime, nowTime))
				{
					maintenanceData.IsEnable = false;
				}

				if (true == TimeUtil.IsPastTime(nowTime, maintenanceData.EndTime))
				{
					maintenanceData.IsEnable = false;
				}

				if (true == maintenanceData.IsEnable)
				{
					isMaintenanceCheck = true;

					if (true == _maintenanceService.ExitWhiteList(uid))
					{
						isMaintenanceCheck = false;
					}
				}
			}

			if (true == isMaintenanceCheck)
			{
				throw new GameErrorException(GameErrorCode.Maintenance, "Maintenance mode", maintenanceData.Contents);
			}

			var accessToken = new AccessToken
			{
				UserId = dboIdentityUserLocal.UserId,
				IdentityType = Type_Identity.Local,
				Uid = uid,
				SignedDate = nowTime,
				SignedUnixTime = TimeUtil.DateTimeToUnixTime(nowTime),
				ExpireTime = nowTime.AddDays(ServerConfigurationManager.TokenExpireAddDate),
			};

			var accessTokenString = _gameSessionService.MakeTokenString(accessToken);
			var refreshToken = _gameSessionService.GenerateRefreshToken();
			var refreshTokenExpiry = nowTime.AddDays(ServerConfigurationManager.RefreshTokenExpiryAddDay);

			// 신규유저인 경우
			if (true == isNewUser)
			{
				if (false == await _dbIdentity.CreateIdentityAsync(accessToken.Uid, accessTokenString, refreshToken, refreshTokenExpiry))
				{
					throw new GameErrorException(GameErrorCode.DBError, "identity insert error.");
				}

				if (false == _dbUserLocal.UpdateUid(dboIdentityUserLocal.UserId, accessToken.Uid))
				{
					throw new GameErrorException(GameErrorCode.DBError, $"user uid update error. {dboIdentityUserLocal.UserId}, {accessToken.Uid}");
				}
			}
			else
			{
				var dboIdentity = _dbIdentity.GetIdentity(dboIdentityUserLocal.Uid);

				// 기존가입자이지만 Uid는 존재하고 Identity가 없는경우 추가로 넣어준다.
				// TBL_UserLocal에는 Uid가 설정되어 있고 TBL_Identity에는 없는 경우
				if (null == dboIdentity)
				{
					if (false == _dbUserLocal.UpdateUid(dboIdentityUserLocal.UserId, accessToken.Uid))
					{
						throw new GameErrorException(GameErrorCode.DBError, "identity userlocal update error.");
					}

					if (false == await _dbIdentity.CreateIdentityAsync(accessToken.Uid, accessTokenString, refreshToken, refreshTokenExpiry))
					{
						throw new GameErrorException(GameErrorCode.DBError, "identity insert error.");
					}
				}
				else
				{
					if (false == await _dbIdentity.UpdateIdentity_AccessTokenAsync(accessToken.Uid, accessTokenString))
					{
						throw new GameErrorException(GameErrorCode.DBError, $"identity update accesstoken error. {accessToken.Uid}");
					}
				}
			}

			_logger.LogDebug($"[{IPAddress}] [HttpREQ_AuthLocal] [uid:{uid}/UserId:{accessToken.UserId}]");

			return new HttpRES_AuthLocal
			{
				Uid = accessToken.Uid,
				ServerTime = TimeUtil.Now,
				AccessToken = accessTokenString,
				Authtime = accessToken.SignedDate,
			};
		}
	}
}
