using Microsoft.Extensions.Logging;

namespace Framework.Core.Services
{
    public sealed class GameSessionService
    {
        private readonly ILogger<GameSessionService> _logger;

        public GameSessionService(ILogger<GameSessionService> logger)
        {
            _logger = logger;
		}
    }
}
