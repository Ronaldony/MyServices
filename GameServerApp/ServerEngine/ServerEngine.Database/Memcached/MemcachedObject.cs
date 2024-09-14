using Enyim.Caching;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.Memcached
{
	using ServerEngine.Database.Interfaces;

	/// <summary>
	/// MemcachedObject.
	/// </summary>
	public sealed class MemcachedObject : ICacheObject
	{
		private ILogger<MemcachedObject> _logger;

		private readonly IMemcachedClient _memcachedClient;

		// 캐싱 유지 시간 초. 3600 sec = 1시간.
		private const int CACHING_SEC = 3600;

		public MemcachedObject(ILogger<MemcachedObject> logger, IMemcachedClient memcachedClient)
		{
			_logger = logger;

			_memcachedClient = memcachedClient;
		}

		/// <summary>
		/// Get data from cache.
		/// </summary>
		public T Get<T>(string key) where T : class
		{
			var data = _memcachedClient.Get<T>(key);

			if (null == data)
			{
				return null;
			}

			return data;
		}

		/// <summary>
		/// Set data into cache.
		/// </summary>
		public bool Set<T>(string key, T value) where T : class
		{
			return _memcachedClient.Set(key, value, 3600);
		}
	}
}