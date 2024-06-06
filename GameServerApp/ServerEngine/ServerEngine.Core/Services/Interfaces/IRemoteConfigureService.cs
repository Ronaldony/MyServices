

namespace ServerEngine.Core.Services.Interfaces
{
    /// <summary>
    /// Interface for remote configure service.
    /// </summary>
    public interface IRemoteConfigureService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get configuration data.
        /// </summary>
        Task<T> GetConfigData<T>(string key) where T : class;
    }
}
