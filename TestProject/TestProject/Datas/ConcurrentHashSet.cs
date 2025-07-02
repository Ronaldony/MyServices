namespace TestProject.Datas
{
    // Thread-Safe HashSet
    /// <summary>
    /// ConcurrentHashSet.
    /// </summary>
    public class ConcurrentHashSet<T> : IDisposable
    {
        private readonly HashSet<T> _hashSet = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public bool Add(T item)
        {
            _lock.EnterWriteLock();
            try { return _hashSet.Add(item); }
            finally { _lock.ExitWriteLock(); }
        }

        public bool TryRemove(T item)
        {
            _lock.EnterWriteLock();
            try { return _hashSet.Remove(item); }
            finally { _lock.ExitWriteLock(); }
        }

        public T[] ToArray()
        {
            _lock.EnterReadLock();
            try { return _hashSet.ToArray(); }
            finally { _lock.ExitReadLock(); }
        }

        public void Dispose() => _lock?.Dispose();
    }

}
