
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Datas.Player;
    using GameCore.Http.Auth;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;

    /// <summary>
    /// 플레이어 생성 컨트롤러.
    /// 새로운 플레이어(캐릭터)를 생성.
    /// </summary>
    public class CreatePlayerController : GameController
    {
        private readonly IUniqueIdService _uniqueIdService;

        private readonly DBIdentity _dbIdentity;
        private readonly DBUser _dbUser;
        private readonly DBPlayerData _dbPlayerData;

        public CreatePlayerController(
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            // services.
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();

            // DB.
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
            _dbUser = serviceProvider.GetRequiredService<DBUser>();
            _dbPlayerData = serviceProvider.GetRequiredService<DBPlayerData>();
        }

        [HttpPost]
        [Route("player/create")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_CreatePlayer> ProcessAsync([FromBody] REQ_CreatePlayer req)
        {
            var nowTime = TimeUtil.Now;

            // 유저 정보 검사.
            var dboUser = await _dbUser.SelectUserOrDefualtAsync(req.UserId);

            if (default == dboUser)
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotExistUserId, $"not exist user. {req.UserId}");
            }

            // Uid 검사.
            var uid = dboUser.Uid;

            if (false == req.Uid.Equals(uid))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotMatchUid, $"not match uid. {req.UserId}");
            }

            // Identity 검사.
            var dboIdentity = await _dbIdentity.SelectIdentityOrDefaultAsync(uid);

            if (default == dboIdentity)
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotExistIdentity, $"not exist identity. {req.UserId}");
            }

            // Token 검사.
            if (TimeUtil.IsPastTime(nowTime, dboIdentity.TokenExpireTime))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_ExpiredToken, $"expire accesstoken.");
            }

            if (false == dboIdentity.Token.Equals(req.AccessToken))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotMatchToken, $"not match accesstoken.");
            }

            // Pid 발급.
            var newPid = _uniqueIdService.GenerateId();

            // Create PlayerData.
            if (false == await _dbPlayerData.CreatePlayerDataAsync(uid, newPid, newPid))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotCreatedPlayer, $"create PlayerData error.");
            }

            // PlayerInfoObject.
            var playerInfoObejct = GetObject<PlayerInfoObject>();
            var newPlayerInfo = new DTO_PlayerInfo();

            // PlayerInfo 초기화.
            newPlayerInfo.Uid = uid;
            newPlayerInfo.Pid = newPid;
            newPlayerInfo.RegTime = nowTime;

            // PlayerInfo 저장.
            if (false == await playerInfoObejct.UpsertAsync(newPid, newPlayerInfo))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotMatchToken, $"upsert PlayerInfo error.");
            }

            return new RES_CreatePlayer
            {
                Uid = uid,
                PlayerInfo = newPlayerInfo
            };
        }
    }
}
