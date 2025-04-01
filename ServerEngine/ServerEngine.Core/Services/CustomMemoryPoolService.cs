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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in assemblies)
            {
                // IMemoryPool 클래스 추가.
                var types = asm.GetTypes().Where(t => t.GetInterface(typeof(IMemoryPool).Name) != null);

                foreach (var type in types)
                {
                    _memoryPoolDict.AddOrUpdate(type, new CustomMemoryPool(), (key, old) => new CustomMemoryPool());
                }
            }

            _logger.LogInformation($"CustomMemoryPoolService initialized.");
		}

		/// <summary>
		/// Acquire object pool.
		/// </summary>
		public T Rent<T>() where T : class, IMemoryPool, new()
		{
            return _memoryPoolDict[typeof(T)].Rent<T>();
		}

		/// <summary>
		/// Release object pool.
		/// </summary>
		public void Release<T>(T obj) where T : class, IMemoryPool, new()
        {
            _memoryPoolDict[typeof(T)].Release(obj);
        }
    }
}
