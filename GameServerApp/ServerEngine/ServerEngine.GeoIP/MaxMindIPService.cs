using MaxMind.GeoIP2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.GeoIP
{
	public class MaxMindIPService : IGeoIPService
	{
		private readonly ILogger<MaxMindIPService> _logger;

		private readonly DatabaseReader _reader;

		public MaxMindIPService(IServiceProvider serviceProvider)
		{
			_logger = serviceProvider.GetRequiredService<ILogger<MaxMindIPService>>();

			var baseDir = AppDomain.CurrentDomain.BaseDirectory;
			_reader = new DatabaseReader($"{AppDomain.CurrentDomain.BaseDirectory}MaxMind/GeoLite2-Country.mmdb");
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		public void Initialize()
		{
			// Initialize.
			_logger.LogInformation("MaxMindIPService Initialized.");
		}

		/// <summary>
		/// GetCountryCode.
		/// Desc: IP로부터 국가 코드를 반환하는 프로세스.
		/// </summary>
		/// <param name="ip">IP address.</param>
		/// <returns>Country code.</returns>
		public string GetCountryCode(string ip)
		{
			return _reader.Country(ip).Country.Name;
		}
	}
}
