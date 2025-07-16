using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.Core.Services
{
    /// <summary>
    /// FirstCreateService.
    /// </summary>
    public class FirstCreateService
    {
        private readonly ILogger<FirstCreateService> _logger;

        private readonly IServiceProvider _serviceProvider;

        public FirstCreateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _logger = serviceProvider.GetRequiredService<ILogger<FirstCreateService>>();
        }

        public void Initialize()
        {
            _logger.LogInformation($"FirstCreateService initialized.");
        }
    }
}
