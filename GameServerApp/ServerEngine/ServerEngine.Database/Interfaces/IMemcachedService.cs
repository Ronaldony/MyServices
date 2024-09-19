
namespace ServerEngine.Database.Interfaces
{
	using Enyim.Caching.Memcached;
	using ServerEngine.Database.Cache;

	/// <summary>
	/// IMemcachedService.
	/// </summary>
	public interface IMemcachedService
	{
		/// <summary>
		/// Initialize.
		/// </summary>
		bool Initialize(IServiceProvider serviceProvider, IEnumerable<CacheHost> cacheHosts, int expireSec);

		/// <summary>
		/// Get from cache.
		/// </summary>
		T Get<T>(string key);

		/// <summary>
		/// Get data with CAS.
		/// </summary>
		CasResult<T> GetWithCas<T>(string key);

		/// <summary>
		/// Set intto cache.
		/// </summary>
		bool Set<T>(string key, T value);

		/// <summary>
		/// Set data with CAS.
		/// </summary>
		bool SetWithCas<T>(string key, T value, ulong cas);

		/// <summary>
		/// Remove.
		/// </summary>
		bool Remove(string key);
	}
}
