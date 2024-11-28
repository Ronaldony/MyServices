using Microsoft.AspNetCore.Mvc;

namespace ServerEngine.Test.Controllers.Test
{
	using ServerEngine.GeoIP;

	/// <summary>
	/// Test_GeoIPController.
	/// </summary>
	public class Test_GeoIPController
	{
		private readonly ILogger<Test_GeoIPController> _logger;

		private readonly IGeoIPService _geoIPService;

		public Test_GeoIPController(IGeoIPService geoIPService)
		{
			_geoIPService = geoIPService;
		}

		/// <summary>
		/// Test Contry match.
		/// </summary>
		[HttpGet]
		[Route("test-geoip/country-match")]
		public string Test_CountryMatch()
		{
			var country = _geoIPService.GetCountryCode("61.39.208.20");

			return default;
		}

		/// <summary>
		/// Test multi-thread safe.
		/// </summary>
		[HttpGet]
		[Route("test-geoip/multi-thread")]
		public string Test_MultiThread()
		{

			return default;
		}
	}
}
