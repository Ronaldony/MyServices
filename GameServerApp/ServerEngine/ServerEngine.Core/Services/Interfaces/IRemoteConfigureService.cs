
namespace ServerEngine.Core.Services.Interfaces
{
    using ServerEngine.Config;

    /// <summary>
    /// Interface for remote configure service.
    /// </summary>
    public interface IRemoteConfigureService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        bool Initialize(ConfigBase config);

        /// <summary>
        /// Get configuration data.
        /// </summary>
        Task<T> GetConfigData<T>(string key) where T : class;
    }
}
