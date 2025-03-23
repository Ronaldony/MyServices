using System;
using System.Collections.Concurrent;

namespace Externals.MemoryPool
{

    /// <summary>
    /// CustomMemoryPoolService.
    /// </summary>
    public sealed class CustomMemoryPoolProvider : Singleton<CustomMemoryPoolProvider>
    {
        private readonly ConcurrentDictionary<Type, CustomMemoryPool> _memoryPoolDict;

        public CustomMemoryPoolProvider()
        {
            _memoryPoolDict = new ConcurrentDictionary<Type, CustomMemoryPool>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
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
