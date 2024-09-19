
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
	public sealed class MemcachedService : IMemcachedService
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
		public bool Initialize(IServiceProvider serviceProvider, IEnumerable<CacheHost> cacheHosts, int expireSec)
		{
			try
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
					_logger.LogInformation($"Memcached server add - {host.Address}:{host.Port}");
				}

				// Memcached configuration.
				var config = new MemcachedClientConfiguration(loggerFactory, options);

				_memcachedClient = new MemcachedClient(loggerFactory, config);
				_expireSec = expireSec;

				_logger.LogInformation($"MemcachedService initialized.");

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Get data.
		/// </summary>
		public T Get<T>(string key)
		{
			return _memcachedClient.Get<T>(key);
		}

		/// <summary>
		/// Get data with CAS.
		/// </summary>
		public CasResult<T> GetWithCas<T>(string key)
		{
			return _memcachedClient.GetWithCas<T>(key);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		public bool Set<T>(string key, T value)
		{
			return _memcachedClient.Set(key, value, _expireSec);
		}

		/// <summary>
		/// Set data with CAS.
		/// </summary>
		public bool SetWithCas<T>(string key, T value, ulong cas)
		{
			var result = _memcachedClient.Cas(StoreMode.Set, key, value, cas);
			return result.Result;
		}

		/// <summary>
		/// Remove.
		/// </summary>
		public bool Remove(string key)
		{
			return _memcachedClient.Remove(key);
		}
	}
}