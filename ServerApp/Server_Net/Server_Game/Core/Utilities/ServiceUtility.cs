using Microsoft.Extensions.DependencyInjection;

namespace Server_Game.Core.Utilities
{
    /// <summary>
    /// ServiceUtility.
    /// </summary>
    public static class ServiceUtility
    {
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// GetServiceProvider.
        /// </summary>
        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        /// <summary>
        /// GetService.
        /// </summary>
        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
