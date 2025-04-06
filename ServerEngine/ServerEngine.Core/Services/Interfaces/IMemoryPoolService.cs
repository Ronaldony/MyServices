
namespace ServerEngine.Core.Services.Interfaces
{
    using ServerEngine.Core.MemoryPool;

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
		T Rent<T>() where T : class,  new();

        /// <summary>
        /// Release.
        /// </summary>
        void Release<T>(T obj) where T : class, new();
    }
}
