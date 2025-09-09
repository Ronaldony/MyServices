using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Utilities
{
    /// <summary>
    /// ServiceUtility.
    /// </summary>
    public static class ServiceUtility
    {
        private static IServiceProvider _serviceProvider;

        private static CancellationTokenSource _cts;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _cts = new CancellationTokenSource();
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
        public static T GetService<T>() where T : class
        {
            return _serviceProvider.GetService<T>();
        }


        /// <summary>
        /// Get cancel token.
        /// </summary>
        public static CancellationTokenSource GetCTS()
        {
            return _cts;
        }
    }
}
