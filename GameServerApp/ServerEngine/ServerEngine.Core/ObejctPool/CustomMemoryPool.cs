using System.Collections.Concurrent;

namespace ServerEngine.Core.ObejctPool
{
    using ServerEngine.Core.DataObject;

	/// <summary>
	/// CustomMemoryPool.
	/// </summary>
	internal sealed class CustomMemoryPool
    {
        private readonly ConcurrentStack<object> _memoryPoolStack;

        public CustomMemoryPool()
        {
            _memoryPoolStack = new ConcurrentStack<object>();
        }

        /// <summary>
        /// Get data object from pool.
        /// </summary>
        public T Get<T>() where T : MemoryPoolBase, new ()
        {
            if (false == _memoryPoolStack.TryPop(out object dataObejct))
            {
                return new T();
            }

            return dataObejct as T;
        }

        /// <summary>
        /// Return data object to pool.
        /// </summary>
        public void Return<T>(T dataObject) where T : MemoryPoolBase, new ()
        {
            dataObject.TryReset();

            _memoryPoolStack.Push(dataObject);
        }
    }
}
