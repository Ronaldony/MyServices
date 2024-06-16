using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.Services
{
    using ServerEngine.Database.Services.Interfaces;

    public class BlobDataService : IBlobDataService
    {
        private ILogger<BlobDataService> _logger;

        public BlobDataService(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<BlobDataService>();
        }

        public T Select<T>(string key) where T : class
        {
            return default;
        }

        public bool Update<T>(string key, T data) where T : class
        {

            return default;
        }
    }
}
