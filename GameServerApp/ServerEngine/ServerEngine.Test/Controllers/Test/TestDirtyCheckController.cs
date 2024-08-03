using MemoryPack.Formatters;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication.PgOutput.Messages;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Core.Util;
using ServerEngine.Test.Controllers.Test.Data;
using ServerEngine.Test.Database.Data;
using ServerEngine.Test.Database.DataObject;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace ServerEngine.Test.Controllers.Test
{
    public class TestDirtyCheckController : Controller
    {
        private readonly ILogger<TestDirtyCheckController> _logger;
        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IDataSerializer _dataSerializer;

        public TestDirtyCheckController(
            ILogger<TestDirtyCheckController> logger, 
            PlayerInfoObejct playerInfoObject,
            IDataSerializer dataSerializer)
        {
            _logger = logger;
            _playerInfoObject = playerInfoObject;
            _dataSerializer = dataSerializer;
        }

        

        [HttpGet]
        [Route("test/dirty-check")]
        public string Process()
        {
            var playerInfo = new TestDirtyCheck
            { 
                TestString = "",
                TestLong = 0,
                TestDouble = 0,
                TestString2 = "",
                TestLong2 = 0,
                TestBool = false,
                TestDateTime = DateTime.MinValue,
                TestFloat = 0,
            };
                
            var oriDatabytes = _dataSerializer.Serialize(playerInfo);
            var oriString = string.Empty;

            _logger.LogInformation($"==================================================");
            _logger.LogInformation($"Start object dirty check.");

            for (long cnt = 0; cnt < long.MaxValue; cnt++)
            {
                // 체크 카운트 131072마다 출력.
                if ((cnt & 0x1FFFF) == 0x1FFFF)
                {
                    _logger.LogInformation($"Check count: {cnt}, Time: {TimeUtil.Now}");

                    TimeUtil.AddNowTime(20);
                    _logger.LogInformation($"now Time: {TimeUtil.Now}");
                }

                // TestString dirty check.
                playerInfo.TestString = GetNextString(playerInfo.TestString);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestString: {playerInfo.TestString}");
                }

                // TestLong dirty check.
                playerInfo.TestLong++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestLong: {playerInfo.TestLong}");
                }

                // TestDouble dirty check.
                playerInfo.TestDouble++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestDouble: {playerInfo.TestDouble}");
                }

                // TestString2 dirty check.
                playerInfo.TestString2 = GetNextString(playerInfo.TestString2);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestString2: {playerInfo.TestString2}");
                }

                // TestLong2 dirty check.
                playerInfo.TestLong2++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestLong2: {playerInfo.TestLong2}");
                }

                // TestBool dirty check.
                playerInfo.TestBool = !playerInfo.TestBool;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestBool: {playerInfo.TestBool}");
                }

                // TestDateTime dirty check.
                playerInfo.TestDateTime = playerInfo.TestDateTime.AddTicks(1);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestDateTime: {playerInfo.TestDateTime}");
                }

                // TestFloat dirty check.
                playerInfo.TestFloat++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    _logger.LogError($"error count: {cnt}, TestFloat: {playerInfo.TestFloat}");
                }
            }

            _logger.LogInformation($"End object dirty check.");
            _logger.LogInformation($"==================================================");

            return "Ok";
        }

        private string GetNextString(string src)
        {
            var bytes = Encoding.UTF8.GetBytes(src);

            if (bytes.Length == 0)
            {
                bytes = new byte[8];
                bytes[7]++;
            }

            var convertLong = BitConverter.ToInt64(bytes, 0);
            convertLong++;  // next number.

            var nextBytes = BitConverter.GetBytes(convertLong);
            return Encoding.UTF8.GetString(nextBytes);
        }

        /// <summary>
        /// Object dirty check.
        /// </summary>
        private bool IsDirty(byte[] ori, byte[] dst)
        {
            if (false == ori.Length.Equals(dst.Length))
            {
                return true;
            }

            var longSize = ori.Length / sizeof(long);
            var oriLong = Unsafe.As<long[]>(ori);
            var dstLong = Unsafe.As<long[]>(dst);

            for (int i = 0; i < longSize; i++)
            {
                if (false == oriLong[i].Equals(dstLong[i]))
                {
                    return true;
                }
            }

            for (int i = longSize * 8; i < ori.Length; i++)
            {
                if (false == ori[i].Equals(dst[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
