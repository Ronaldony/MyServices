namespace ServerEngine.Core.Services.Interfaces
{
    /// <summary>
    /// Object pool interface.
    /// </summary>
    public interface IObjectPoolService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Acquire.
        /// </summary>
        T Acquire<T>() where T: class, new ();

        /// <summary>
        /// Release.
        /// </summary>
        bool Release<T>(T obj) where T: class, new ();
    }
}
