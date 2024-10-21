using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Test.Database.Data;

namespace ServerEngine.Test.Controllers.Test
{
    /// <summary>
    /// Test_ObjectPoolController.
    /// </summary>
    public class Test_ObjectPoolController : ControllerBase
    {
        private readonly ILogger<Test_ObjectPoolController> _logger;

        private readonly IObjectPoolService _objectPoolService;

        public Test_ObjectPoolController(
            ILogger<Test_ObjectPoolController> logger,
            IObjectPoolService objectPoolService
            )
        {
            _logger = logger;
            _objectPoolService = objectPoolService;
        }

        [HttpGet]
        [Route("test-objectpool/working")]
        public string Test_Working()
        {
            TestCounter.ObjectPool_Acquire = 0;
            TestCounter.ObjectPool_Release = 0;

            var playerInfos = new List<DTO_PlayerInfo>();

            for (int cnt2 = 0; cnt2 < 10; cnt2++)
            {
                int start = 0;

                for (int cnt = 0; cnt < 400000; cnt++)
                {
                    playerInfos.Add(_objectPoolService.Acquire<DTO_PlayerInfo>());
                }

                foreach (var playerInfo in playerInfos)
                {
                    _objectPoolService.Release(playerInfo);
                }

                playerInfos.Clear();
            }

            return $"Acquire count: {TestCounter.ObjectPool_Acquire} / Release count: {TestCounter.ObjectPool_Release}";
        }

        [HttpGet]
        [Route("test-objectpool/multi-thread")]
        public string Test_ThreadSafe()
        {
            return default;
        }

        [HttpGet]
        [Route("test-objectpool/speed")]
        public string Test_Spped()
        {
            return default;
        }
    }
}
