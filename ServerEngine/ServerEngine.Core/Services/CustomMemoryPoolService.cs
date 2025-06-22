using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.MemoryPool;
    using ServerEngine.Core.Services.Interfaces;

	/// <summary>
	/// CustomMemoryPoolService.
	/// </summary>
	public sealed class CustomMemoryPoolService : IMemoryPoolService
    {
		private readonly ILogger<CustomMemoryPoolService> _logger;

        private readonly ConcurrentDictionary<Type, CustomMemoryPool> _memoryPoolDict;

        public CustomMemoryPoolService(IServiceProvider serviceProvider)
        {
			_logger = serviceProvider.GetRequiredService<ILogger<CustomMemoryPoolService>>();

			_memoryPoolDict = new ConcurrentDictionary<Type, CustomMemoryPool>();
        }

		/// <summary>
		/// Initialize.
		/// </summary>
		public void Initialize()
        {
            _logger.LogInformation($"CustomMemoryPoolService initialized.");
		}

		/// <summary>
		/// Acquire object pool.
		/// </summary>
		public T Rent<T>() where T : class, IDisposable, new()
        {
            if (false == _memoryPoolDict.TryGetValue(typeof(T), out var pool))
            {
                _memoryPoolDict.TryAdd(typeof(T), new CustomMemoryPool());

                return new T();
            }

            return pool.Rent<T>();
        }

		/// <summary>
		/// Release object pool.
        /// Release 후 Dispose 수행.
		/// </summary>
		public void Release<T>(T obj) where T : class, IDisposable, new()
        {
            if (false == _memoryPoolDict.TryGetValue(typeof(T), out var pool))
            {
                _memoryPoolDict.TryAdd(typeof(T), new CustomMemoryPool());
            }

            // Dispose.
            obj.Dispose();

            _memoryPoolDict[typeof(T)].Release(obj);
        }
    }
}
