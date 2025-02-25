using System.Collections.Concurrent;

namespace ServerEngine.Core.ObejctPool
{
	/// <summary>
	/// CustomMemoryPool.
	/// </summary>
	internal sealed class CustomMemoryPool
    {
        private readonly ConcurrentStack<object> _memoryPoolStack;

        public CustomMemoryPool(Type type)
        {
            _memoryPoolStack = new ConcurrentStack<object>();
		}

        /// <summary>
        /// Get data object from pool.
        /// </summary>
        public T Rent<T>() where T : class, new ()
        {
            if (false == _memoryPoolStack.TryPop(out var dataObejct))
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
            _memoryPoolStack.Push(dataObject);
        }
    }
}
