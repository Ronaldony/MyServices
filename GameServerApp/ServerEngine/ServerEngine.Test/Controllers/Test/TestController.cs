using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;
using ServerEngine.Test.Database.Data;
using ServerEngine.Test.Database.DataObject;
using System.Runtime.CompilerServices;

namespace ServerEngine.Test.Controllers.Test
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IDataSerializer _dataSerializer;

        public TestController(
            ILogger<TestController> logger, 
            PlayerInfoObejct playerInfoObject,
            IDataSerializer dataSerializer)
        {
            _logger = logger;
            _playerInfoObject = playerInfoObject;
            _dataSerializer = dataSerializer;
        }

        [HttpGet]
        [Route("test")]
        [Route("")]
        public string Process()
        {
            var isUpsert = _playerInfoObject.Upsert("Test", new DTO_PlayerInfo
            { 
                Pid = "Test pid",
                PlayerName = "Test name",
                RegTime = TimeUtil.Now,
            });
                
            var playerInfo = _playerInfoObject.Select<DTO_PlayerInfo>("Test");

            return "Ok";
        }
    }
}
