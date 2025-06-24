
namespace ServerEngine.Core.Services.Interfaces
{
    /// <summary>
    /// Object pool interface.
    /// </summary>
    public interface IMemoryPoolService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Acquire.
        /// </summary>
		T Rent<T>() where T : class, IDisposable, new();

        /// <summary>
        /// Release.
        /// </summary>
        void Release<T>(T obj) where T : class, IDisposable, new();

        /// <summary>
        /// Release.
        /// </summary>
        void Release<T>(ICollection<T> obj) where T : class, IDisposable, new();
    }
}
