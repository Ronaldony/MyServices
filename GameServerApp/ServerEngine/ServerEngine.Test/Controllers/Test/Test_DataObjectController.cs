using Microsoft.AspNetCore.Mvc;

namespace ServerEngine.Test.Controllers.Test
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;
    using ServerEngine.Test.Database.Data;
    using ServerEngine.Test.Database.DataObject;

    /// <summary>
    /// Test_Controller.
    /// </summary>
    public class Test_DataObjectController : Controller
    {
        private readonly ILogger<Test_DataObjectController> _logger;
        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IDataSerializer _dataSerializer;

        public Test_DataObjectController(
            ILogger<Test_DataObjectController> logger, 
            PlayerInfoObejct playerInfoObject,
            IDataSerializer dataSerializer)
        {
            _logger = logger;
            _playerInfoObject = playerInfoObject;
            _dataSerializer = dataSerializer;
        }

        [HttpGet]
        [Route("test")]
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
