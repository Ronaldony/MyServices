using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text;

namespace ServerEngine.Test.Controllers.Test
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;
    using ServerEngine.Test.Controllers.Test.Data;
    using ServerEngine.Test.Database.DataObject;

    /// <summary>
    /// Test_DirtyCheckController.
    /// </summary>
	public class Test_DirtyCheckController : Controller
    {
        private readonly ILogger<Test_DirtyCheckController> _logger;
        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IDataSerializer _dataSerializer;

        public Test_DirtyCheckController(
            ILogger<Test_DirtyCheckController> logger, 
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
            var playerInfo = new TestData
            { 
                Data_Str1 = "",
                Data_Long1 = 0,
                Data_Double1 = 0,
                Data_Str2 = "",
                Data_Long2 = 0,
                Data_Bool1 = false,
                Data_Date1 = DateTime.MinValue,
                Data_Float1 = 0,
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
                }

                // TestString dirty check.
                playerInfo.Data_Str1 = GetNextString(playerInfo.Data_Str1);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestString: {playerInfo.Data_Str1}";
                }

                // TestLong dirty check.
                playerInfo.Data_Long1++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestLong: {playerInfo.Data_Long1}";
                }

                // TestDouble dirty check.
                playerInfo.Data_Double1++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestDouble: {playerInfo.Data_Double1}";
                }

                // TestString2 dirty check.
                playerInfo.Data_Str2 = GetNextString(playerInfo.Data_Str2);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestString2: {playerInfo.Data_Str2}";
                }

                // TestLong2 dirty check.
                playerInfo.Data_Long2++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestLong2: {playerInfo.Data_Long2}";
                }

                // TestBool dirty check.
                playerInfo.Data_Bool1 = !playerInfo.Data_Bool1;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestBool: {playerInfo.Data_Bool1}";
                }

                // TestDateTime dirty check.
                playerInfo.Data_Date1 = playerInfo.Data_Date1.AddTicks(1);
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestDateTime: {playerInfo.Data_Date1}";
                }

                // TestFloat dirty check.
                playerInfo.Data_Float1++;
                if (false == IsDirty(_dataSerializer.Serialize(playerInfo), oriDatabytes))
                {
                    return $"error count: {cnt}, TestFloat: {playerInfo.Data_Float1}";
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
