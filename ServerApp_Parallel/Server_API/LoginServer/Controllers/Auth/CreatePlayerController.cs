
using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;

namespace LoginServer.Controllers.Auth
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DataObjects;
    using Framework.Core.WebAPI.ActionFilter;
    using Framework.WebAPI;
    using Framework.WebAPI.Controllers;
    using GameCore;
    using GameCore.Http.Auth;

    /// <summary>
    /// 플레이어 생성 컨트롤러.
    /// 새로운 플레이어(캐릭터)를 생성.
    /// </summary>
    public class CreatePlayerController : GameController
    {
        private readonly ILogger<CreatePlayerController> _logger;

        private readonly IUniqueIdService _uniqueIdService;

        private readonly DBPlayerData _dbPlayerData;
        private readonly DBIdentity _dbIdentity;

        public CreatePlayerController(
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CreatePlayerController>>();

            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();

            _dbPlayerData = serviceProvider.GetRequiredService<DBPlayerData>();
            _dbIdentity = serviceProvider.GetRequiredService<DBIdentity>();
        }

        [HttpPost]
        [Route("auth/create_player")]
        [ServiceFilter(typeof(AuthActionFilter))]
        public async Task<RES_CreatePlayer> ProcessAsync([FromBody] REQ_CreatePlayer req)
        {
            var nowTime = TimeUtil.Now;

            // Pid 발급.
            var newPid = _uniqueIdService.GenerateId();
            var playerName = newPid;

            // Update controller.
            base.Pid = newPid;

            // Create PlayerData.
            if (false == await _dbIdentity.UpdatePidAsync(base.Uid, newPid))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_UpdatePid, $"update pid error.");
            }

            // Create PlayerData.
            if (false == await _dbPlayerData.Create(base.Uid, newPid, playerName))
            {
                throw new ServerException(GameErrorCode.CreatePlayer_NotCreatedPlayer, $"create PlayerData error.");
            }

            // 플레이어 객체 생성.
            await CreateOrUpdatePlayerAsync(base.Uid, newPid, nowTime);

            _logger.LogDebug($"Create player. pid: {newPid}, player name: {playerName}");

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
