using Microsoft.AspNetCore.Mvc;

namespace ServerEngine.Test.Controllers
{
    using GameCore.Datas.Player;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;
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
        [Route("test-dataobject")]
        public async Task<string> Process()
        {
            var isUpsert = await _playerInfoObject.UpsertAsync("TestPid", new DTOPlayerInfo
            { 
                Pid = "TestPid",
                PlayerName = "Test name",
                RegTime = TimeUtil.Now,
            });
                
            var playerInfo = await _playerInfoObject.SelectAsync<DTOPlayerInfo>("TestPid");

            return "Ok";
        }
    }
}
