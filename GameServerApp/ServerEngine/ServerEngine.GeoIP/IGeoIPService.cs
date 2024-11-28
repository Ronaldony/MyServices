namespace ServerEngine.GeoIP
{
	public interface IGeoIPService
	{
		/// <summary>
		/// Initialize.
		/// </summary>
		public void Initialize();

		/// <summary>
		/// GetCountryCode.
		/// Desc: IP로부터 국가 코드를 반환하는 프로세스.
		/// </summary>
		/// <param name="ip">IP address.</param>
		/// <returns>Country code.</returns>
		string GetCountryCode(string ip);
	}
}
