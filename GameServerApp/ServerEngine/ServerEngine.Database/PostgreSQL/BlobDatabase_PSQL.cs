using Microsoft.Extensions.Logging;
using ServerEngine.Database.Interfaces;

namespace ServerEngine.Database.PostgreSQL
{
    public sealed class BlobDatabase_PSQL : IBlobDatabase
    {
        private readonly ILogger<BlobDatabase_PSQL> _logger;
        private readonly IBlobDatabaseService _blobDBService;

        public BlobDatabase_PSQL(ILogger<BlobDatabase_PSQL> logger, IBlobDatabaseService blobDBService)
        {
            _logger = logger;
            _blobDBService = blobDBService;
        }

        public T Select<T>(string key) where T : class
        {
            return null;
        }

        public bool Upsert<T>(string key, T data) where T : class
        {
            return true;
        }
    }
}
