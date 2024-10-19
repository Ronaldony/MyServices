using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Services.Interfaces;

namespace ServerEngine.Test.Controllers.Test
{
    /// <summary>
    /// Test_ObjectPoolController.
    /// </summary>
    public class Test_ObjectPoolController : Controller
    {
        private readonly ILogger<Test_ObjectPoolController> _logger;

        private readonly IObjectPoolService _objectPoolService;

        [HttpGet]
        [Route("test-objectpool/working")]
        public string Test_Working()
        {
            return default;
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
