using System;
using System.Collections.Concurrent;

namespace Externals.MemoryPool
{
    /// <summary>
    /// CustomMemoryPool.
    /// </summary>
    internal sealed class CustomMemoryPool
    {
        private readonly ConcurrentQueue<object> _memoryPoolStack;

        public CustomMemoryPool(Type type)
        {
            _memoryPoolStack = new ConcurrentQueue<object>();
		}

        /// <summary>
        /// Get data object from pool.
        /// </summary>
        public T Rent<T>() where T : class, new ()
        {
            if (false == _memoryPoolStack.TryDequeue(out var dataObejct))
            {
                return new T();
            }

            return dataObejct as T;
        }

        /// <summary>
        /// Return data object to pool.
        /// </summary>
        public void Release<T>(T dataObject) where T : class, new ()
        {
            _memoryPoolStack.Enqueue(dataObject);
        }
    }
}
