
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObject;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Http.Auth;
    using Microsoft.AspNetCore.Mvc;

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

            // Pid 발급.
            var newPid = _uniqueIdService.GenerateId();
            var playerName = newPid;

            // Create PlayerData.
            if (false == await _dbPlayerData.CreatePlayerDataAsync(base.Uid, newPid, playerName))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotCreatedPlayer, $"create PlayerData error.");
            }

            // 플레이어 객체 생성.
            await CreateOrUpdatePlayerAsync(base.Uid, newPid, nowTime);

            return new RES_CreatePlayer
            {
                Uid = base.Uid,
                Pid = newPid,
                PlayerName = playerName,
            };
        }

        /// <summary>
        /// Create or update player.
        /// </summary>
        private async Task CreateOrUpdatePlayerAsync(string uid, string pid, DateTime nowTime)
        {
            // PlayInfo.
            var playInfoObject = await GetObjectAsync<PlayInfoObject>();
            var playInfo = playInfoObject.PlayInfo;

            playInfo.Pid = pid;
            playInfo.PlayerName = pid;
            playInfo.UpdateTime = nowTime;
            playInfo.Location.Area = Type_Area.HuntZone;
            playInfo.Stat.Velocity = 4;
            playInfo.Equipment.Weapon = 1;

            // InventoryInfo.
            var inventoryInfoObject = await GetObjectAsync<InventoryInfoObject>();

            // PlayerInfoObject.
            var playerInfoObejct = await GetObjectAsync<PlayerInfoObject>();
            var playerInfo = playerInfoObejct.PlayerInfo;

            playerInfo.Uid = uid;
            playerInfo.Pid = pid;
            playerInfo.RegTime = nowTime;
        }
    }
}
