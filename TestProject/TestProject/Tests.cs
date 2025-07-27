using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace TestProject
{
    /// <summary>
    /// Tests.
    /// </summary>
    public class Tests
    {
        public string Name { get; set; }
        
        public int Id { get; set; }
    }

    public class RefMemory<T> : IDisposable
        where T : class, new ()
    {
        public RefMemory(T data)
        {
            Data = data;
        }

        public void AddRef()
        {
            Interlocked.Increment(ref _refCount);
        }

        public void DecRef()
        {
            Interlocked.Decrement(ref _refCount);
        }

        public void Dispose()
        {
            DecRef();
        }


        public T Data { get; set; }

        // 참조 카운트.
        private int _refCount;
    }
}
