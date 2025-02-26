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
		public T Rent<T>() where T : class, new()
		{
			if (false == _memoryPoolDict.TryGetValue(typeof(T), out _))
			{
				var type = typeof(T);
				_memoryPoolDict.TryAdd(type, new CustomMemoryPool(type));
			}

			return _memoryPoolDict[typeof(T)].Rent<T>();
		}

		/// <summary>
		/// Release object pool.
		/// </summary>
		public bool Release<T>(T obj) where T : class, new()
        {
            try
            {
                _memoryPoolDict[typeof(T)].Release(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
