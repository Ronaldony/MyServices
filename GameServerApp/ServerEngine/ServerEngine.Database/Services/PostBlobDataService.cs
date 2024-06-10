using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.Services
{
    using ServerEngine.Database.Services.Interfaces;

    public class PostBlobDataService : IBlobDataService
    {
        private ILogger<PostBlobDataService> _logger;

        public PostBlobDataService(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<PostBlobDataService>();
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
