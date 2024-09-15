using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ServerEngine.Test.Controllers.Test
{
	using ServerEngine.Core.Util;
	using ServerEngine.Database.Interfaces;
	using ServerEngine.Test.Database.Data;
	using ServerEngine.Test.Database.DataObject;

	public class TestCacheController : Controller
    {
        private readonly ILogger<TestCacheController> _logger;

        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly ICacheService _cacheService;

        public TestCacheController(
            ILogger<TestCacheController> logger, 
            PlayerInfoObejct playerInfoObject,
			ICacheService cacheObject)
        {
            _logger = logger;

            _playerInfoObject = playerInfoObject;
            _cacheService = cacheObject;
        }

        [HttpGet]
        [Route("test-cache")]
        public string Process()
        {
            var playerInfo = new DTO_PlayerInfo
            {
                Pid = "Test_Pid",
				PlayerName = "Test name",
				RegTime = TimeUtil.Now,
			};

            var isSet = _cacheService.Set<DTO_PlayerInfo>(playerInfo.Pid, playerInfo);
            if (false == isSet)
            {
                return "failed to set cache";
            }

            var cachedData = _cacheService.Get<DTO_PlayerInfo>(playerInfo.Pid);
            if (null == cachedData)
            {
                return "failed to get cached data";
            }

            return JsonConvert.SerializeObject(cachedData);
        }
    }
}
