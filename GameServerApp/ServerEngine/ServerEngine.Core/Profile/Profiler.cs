using System.Diagnostics;

namespace ServerEngine.Core.Profile
{
    public class Profiler
    {
        public Stopwatch SW { get; set; }

        public int CountSW { get; set; }

        public Profiler()
        {
            SW = new Stopwatch();
        }

        /// <summary>
        /// profile.
        /// </summary>
        public void Profile(Action action)
        {
            SW.Start();

            action();

            SW.Stop();
            CountSW++;
        }

        /// <summary>
        /// Profile async.
        /// </summary>
        public async Task ProfileAsync(Func<Task> func)
        {
            SW.Start();

            await func();

            SW.Stop();
            CountSW++;
        }

        /// <summary>
        /// Reset profiler.
        /// </summary>
        public void Reset()
        {
            SW.Reset();
            CountSW = 0;
        }
    }
}
