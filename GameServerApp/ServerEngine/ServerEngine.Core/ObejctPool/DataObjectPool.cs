using System.Collections.Concurrent;

namespace ServerEngine.Core.ObejctPool
{
    using ServerEngine.Core.DataObject;

    /// <summary>
    /// DataObjectPool.
    /// Description: Data object pool.
    /// </summary>
    internal sealed class DataObjectPool
    {
        private readonly Type _objectType;

        private readonly ConcurrentStack<object> _obejctPoolStack;

        public DataObjectPool(Type type)
        {
            _objectType = type;
            _obejctPoolStack = new ConcurrentStack<object>();
        }

        /// <summary>
        /// Get data object from pool.
        /// </summary>
        public T Get<T>() where T : DataObjectBase, new ()
        {
            if (false == _obejctPoolStack.TryPop(out object dataObejct))
            {
                return new T();
            }

            return dataObejct as T;
        }

        /// <summary>
        /// Return data object to pool.
        /// </summary>
        public void Return<T>(T dataObject) where T : DataObjectBase, new ()
        {
            dataObject.TryReset();

            _obejctPoolStack.Push(dataObject);
        }
    }
}
