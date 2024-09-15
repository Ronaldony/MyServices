
namespace ServerEngine.Database.Interfaces
{
	using ServerEngine.Database.Cache;

	/// <summary>
	/// ICacheService.
	/// </summary>
	public interface ICacheService
	{
		/// <summary>
		/// Initialize.
		/// </summary>
		void Initialize(IServiceProvider serviceProvider, IEnumerable<CacheHost> cacheHosts, int expireSec);

		/// <summary>
		/// Get from cache.
		/// </summary>
		T Get<T>(string key);

		/// <summary>
		/// Set intto cache.
		/// </summary>
		bool Set<T>(string key, T value);
	}
}
