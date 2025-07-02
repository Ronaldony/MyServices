using System.Collections.Concurrent;

namespace TestProject.Datas
{
    // ===== 13. 성능 최적화 도구 =====
    /// <summary>
    /// PerformanceOptimizations.
    /// </summary>
    public static class PerformanceOptimizations
    {
        // 객체 풀링
        public static class ObjectPool<T> where T : class, new()
        {
            private static readonly ConcurrentQueue<T> _objects = new();

            public static T Get() => _objects.TryDequeue(out var item) ? item : new T();
            public static void Return(T item) => _objects.Enqueue(item);
        }

        // 메모리 효율적인 이벤트 버퍼
        public class RingBuffer<T>
        {
            private readonly T[] _buffer;
            private readonly int _size;
            private volatile int _head;
            private volatile int _tail;

            public RingBuffer(int size)
            {
                _size = size;
                _buffer = new T[size];
            }

            public bool TryAdd(T item)
            {
                var newTail = (_tail + 1) % _size;
                if (newTail == _head) return false; // Buffer full

                _buffer[_tail] = item;
                _tail = newTail;
                return true;
            }

            public bool TryTake(out T item)
            {
                item = default!;
                if (_head == _tail) return false; // Buffer empty

                item = _buffer[_head];
                _head = (_head + 1) % _size;
                return true;
            }
        }
    }
}
