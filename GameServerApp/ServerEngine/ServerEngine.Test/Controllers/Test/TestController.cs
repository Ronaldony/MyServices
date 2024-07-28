using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Util;
using ServerEngine.Test.Database.Data;
using ServerEngine.Test.Database.DataObject;

namespace ServerEngine.Test.Controllers.Test
{
    public class TestController : Controller
    {
        private readonly DataObject_PlayerInfo _playerInfoObject;

        public TestController(DataObject_PlayerInfo playerInfoObject)
        {
            _playerInfoObject = playerInfoObject;
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
