using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ServerEngine.Test.Controllers.Test
{
    using ServerEngine.Core.Util;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Test.Database.Data;
    using ServerEngine.Test.Database.DataObject;

    /// <summary>
    /// Test_CacheController.
    /// </summary>
    public class Test_CacheController : Controller
    {
        private readonly ILogger<Test_CacheController> _logger;

        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IMemcachedService _memcachedService;

        public Test_CacheController(
            ILogger<Test_CacheController> logger, 
            PlayerInfoObejct playerInfoObject,
			IMemcachedService memcachedService)
        {
            _logger = logger;

            _playerInfoObject = playerInfoObject;
            _memcachedService = memcachedService;
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

            var key = playerInfo.Pid;

			var isSet = _memcachedService.Set(key, playerInfo);
            if (false == isSet)
            {
                return "failed to set cache";
            }

            var data = _memcachedService.Get<DTO_PlayerInfo>(key);
            if (data == null)
            {
                return "failed to get with cas.";
            }

			return JsonConvert.SerializeObject(data);
        }

		[HttpGet]
		[Route("test-cache-cas")]
		public string ProcessCas()
		{
			var playerInfo = new DTO_PlayerInfo
			{
				Pid = "Test_Pid",
				PlayerName = "Test name",
				RegTime = TimeUtil.Now.AddDays(1),
			};

			var key = playerInfo.Pid;

			var isSet = _memcachedService.Set(key, playerInfo);
			if (false == isSet)
			{
				return "failed to set cache";
			}

			var casResult = _memcachedService.GetWithCas<DTO_PlayerInfo>(key);
			if (casResult.Result == null)
			{
				return "failed to get with cas.";
			}

			// set to fail SetWithCas.
			isSet = _memcachedService.Set(key, playerInfo);
			if (false == isSet)
			{
				return "failed to force to set";
			}

			isSet = _memcachedService.SetWithCas<DTO_PlayerInfo>(key, playerInfo, casResult.Cas);
			if (false == isSet)
			{
				return "failed to set with cas.";
			}

			return JsonConvert.SerializeObject(casResult.Result);
		}

		[HttpGet]
		[Route("test-cache-cas-init")]
		public string ProcessCasInit()
		{
			var playerInfo = new DTO_PlayerInfo
			{
				Pid = "Test_Pid",
				PlayerName = "Test name",
				RegTime = TimeUtil.Now.AddDays(1),
			};

			var key = playerInfo.Pid;

			var isRemoved = _memcachedService.Remove(key);

			var casResult = _memcachedService.GetWithCas<DTO_PlayerInfo>(key);
			if (casResult.Result != null)
			{
				return "cache is not removed.";
			}

			var isSet = _memcachedService.SetWithCas<DTO_PlayerInfo>(key, playerInfo, casResult.Cas);
			if (false == isSet)
			{
				return "failed to set with cas.";
			}

			 casResult = _memcachedService.GetWithCas<DTO_PlayerInfo>(key);
			if (casResult.Result == null)
			{
				return "failed to get with cas.";
			}

			return JsonConvert.SerializeObject(casResult.Result);
		}
	}
}
