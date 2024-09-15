
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.Cache
{
	using ServerEngine.Database.Interfaces;

	/// <summary>
	/// MemcachedObject.
	/// </summary>
	public sealed class MemcachedService : ICacheService
	{
		private ILogger<MemcachedService> _logger;

		private IMemcachedClient _memcachedClient;

		private int _expireSec;

		public MemcachedService(ILogger<MemcachedService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		public void Initialize(IServiceProvider serviceProvider, IEnumerable<CacheHost> cacheHosts, int expireSec)
		{
			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

			// Memcached options.
			var options = new MemcachedClientOptions
			{
				Protocol = MemcachedProtocol.Binary
			};

			foreach (var host in cacheHosts)
			{
				options.AddServer(host.Address, host.Port);
			}

			// Memcached configuration.
			var config = new MemcachedClientConfiguration(loggerFactory, options);

			_memcachedClient = new MemcachedClient(loggerFactory, config);
			_expireSec = expireSec;
		}

		/// <summary>
		/// Get data from cache.
		/// </summary>
		public T Get<T>(string key)
		{
			var data = _memcachedClient.Get<T>(key);

			if (null == data)
			{
				return default;
			}

			return data;
		}

		/// <summary>
		/// Set data into cache.
		/// </summary>
		public bool Set<T>(string key, T value)
		{
			return _memcachedClient.Set(key, value, _expireSec);
		}
	}
}