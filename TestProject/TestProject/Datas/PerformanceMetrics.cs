using System.Collections.Concurrent;

namespace TestProject.Datas
{
    // ===== 2. 성능 메트릭 수집 =====
    /// <summary>
    /// PerformanceMetrics.
    /// </summary>
    public class PerformanceMetrics
    {
        private readonly ConcurrentDictionary<string, long> _counters = new();
        private readonly ConcurrentDictionary<string, (long sum, long count)> _averages = new();

        public void IncrementCounter(string name) => _counters.AddOrUpdate(name, 1, (k, v) => v + 1);

        public void RecordTime(string name, long milliseconds)
        {
            _averages.AddOrUpdate(name, (milliseconds, 1), (k, v) => (v.sum + milliseconds, v.count + 1));
        }

        public void PrintMetrics()
        {
            Console.WriteLine("=== Performance Metrics ===");
            foreach (var (name, count) in _counters)
            {
                Console.WriteLine($"{name}: {count:N0}");
            }
            foreach (var (name, (sum, count)) in _averages)
            {
                Console.WriteLine($"{name} avg: {sum / (double)count:F2}ms");
            }
        }
    }
}
