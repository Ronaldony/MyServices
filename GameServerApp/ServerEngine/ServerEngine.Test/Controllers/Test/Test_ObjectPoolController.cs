using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Profile;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Test.Database.Data;
using System.Diagnostics;
using System.Text;

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
			IObjectPoolService objectPoolService)
		{
			_logger = logger;
			_objectPoolService = objectPoolService;
		}

		/// <summary>
		/// Test_Working.
		/// Desc: DataObjectPoolService로부터 Acquire과 Release가 정상적(=순차적)으로 처리되는지 확인하는 프로세스.
		/// </summary>
		[HttpGet]
		[Route("test-objectpool/working")]
		public string Test_Working()
		{
			TestCounter.ObjectPool_Acquire = 0;
			TestCounter.ObjectPool_Release = 0;

			var playerInfos = new List<DTO_PlayerInfo>();

			for (int cnt2 = 0; cnt2 < 10; cnt2++)
			{
				// Acquire.
				for (int cnt = 0; cnt < 400000; cnt++)
				{
					playerInfos.Add(_objectPoolService.Acquire<DTO_PlayerInfo>());
				}

				// Release.
				foreach (var playerInfo in playerInfos)
				{
					_objectPoolService.Release(playerInfo);
				}

				playerInfos.Clear();
			}

			return $"Acquire count: {TestCounter.ObjectPool_Acquire} / Release count: {TestCounter.ObjectPool_Release}";
		}

		/// <summary>
		/// Test_ThreadSafe.
		/// Desc: DataObjectPoolService의 Acquire과 Release가 Thread safe로 처리되는지 확인하는 프로세스.
		/// </summary>
		[HttpGet]
		[Route("test-objectpool/multi-thread")]
		public string Test_ThreadSafe()
		{
			for (var cnt = 0; cnt < 20; cnt++)
			{
				// 스레드 생성.
				ThreadPool.QueueUserWorkItem(_TestThreadSafe, cnt);
			}

			return default;
		}

		/// <summary>
		/// Acquire & Release DataObejct.
		/// </summary>
		private void _TestThreadSafe(object param)
		{
			var threadId = (int)param;

			_logger.LogInformation($"_TestThreadSafe Start {threadId}");

			var playerInfos = new List<DTO_PlayerInfo>();
			var nowTime = DateTime.Now;

			// Acquire DataObejct.
			for (int cnt = 0; cnt < 1000000; cnt++)
			{
				var playerInfo = _objectPoolService.Acquire<DTO_PlayerInfo>();
				playerInfos.Add(playerInfo);

				playerInfo.Pid = (cnt + threadId).ToString();
				playerInfo.Uid = (cnt + threadId + 1).ToString();
				playerInfo.PlayerName = (cnt + threadId + 2).ToString();
				playerInfo.RegTime = nowTime.AddMilliseconds(cnt + threadId);
			}

			// Valdation value.
			foreach (var pair in playerInfos.Select((value, index) => (value, index)))
			{
				var cnt = pair.index;
				var playerInfo = pair.value;

				// Check Pid.
				if (!playerInfo.Pid.Equals((cnt + threadId).ToString()))
				{
					_logger.LogError($"pid eror. count: {cnt}, threadId: {threadId}");
				}

				// Check Uid.
				if (!playerInfo.Uid.Equals((cnt + threadId + 1).ToString()))
				{
					_logger.LogError($"uid eror. count: {cnt}, threadId: {threadId}");
				}

				// Check PlayerName.
				if (!playerInfo.PlayerName.Equals((cnt + threadId + 2).ToString()))
				{
					_logger.LogError($"playerName eror. count: {cnt}, threadId: {threadId}");
				}

				// Check RegTime.
				if (!playerInfo.RegTime.Equals(nowTime.AddMilliseconds(cnt + threadId)))
				{
					_logger.LogError($"regTime eror. count: {cnt}, threadId: {threadId}");
				}

				_objectPoolService.Release(playerInfo);
			}

			_logger.LogInformation($"_TestThreadSafe End {threadId}");
		}

		[HttpGet]
		[Route("test-objectpool/benchmark-pool")]
		public string Test_BenchmarkPool()
		{
			const int maxBenchmarkCount = 10;
			const int dataObjectCount = 4000000;

			// Acquire DataObejct.
			var dtoBenchmarks = new List<DTO_Benchmark>();
			for (int acquireCount = 0; acquireCount < dataObjectCount; acquireCount++)
			{
				dtoBenchmarks.Add(_objectPoolService.Acquire<DTO_Benchmark>());
			}

			// Release DataObject.
			foreach (var benchmark in dtoBenchmarks)
			{
				_objectPoolService.Release(benchmark);
			}

			dtoBenchmarks.Clear();

			// Create obejct by ObjectPool.
			var sw = new Stopwatch();

			// Acquire DataObejct.
			for (int benchmarkCount = 0; benchmarkCount < maxBenchmarkCount; benchmarkCount++)
			{
				for (int cnt = 0; cnt < dataObjectCount; cnt++)
				{
					sw.Start();

					var dtoBenchmark = _objectPoolService.Acquire<DTO_Benchmark>();

					sw.Stop();

					dtoBenchmarks.Add(dtoBenchmark);
				}

				foreach (var playerInfo in dtoBenchmarks)
				{
					_objectPoolService.Release(playerInfo);
				}

				dtoBenchmarks.Clear();
			}

			var avgMS = sw.ElapsedMilliseconds / (double)maxBenchmarkCount;

			var sb = new StringBuilder();
			sb.AppendLine($"//////////////////////////////////////////////////////////////////");
			sb.AppendLine($"[Benchmark - Object Pool] ms(total): {sw.ElapsedMilliseconds}, ms(avg): {avgMS}");

			return sb.ToString();
		}

		[HttpGet]
		[Route("test-objectpool/benchmark-new")]
		public string Test_BenchmarkNew()
		{
			const int maxBenchmarkCount = 10;
			const int dataObjectCount = 4000000;

			// Create obejct by ObjectPool.
			var sw = new Stopwatch();

			// Acquire DataObejct.
			for (int benchmarkCount = 0; benchmarkCount < maxBenchmarkCount; benchmarkCount++)
			{
				for (int cnt = 0; cnt < dataObjectCount; cnt++)
				{
					sw.Start();

					var playerInfo = new DTO_Benchmark();

					sw.Stop();
				}
			}

			var avgMS = sw.ElapsedMilliseconds / (double)maxBenchmarkCount;

			var sb = new StringBuilder();
			sb.AppendLine($"//////////////////////////////////////////////////////////////////");
			sb.AppendLine($"[Benchmark - New] ms(total): {sw.ElapsedMilliseconds}, ms(avg): {avgMS}");

			return sb.ToString();
		}
	}
}
