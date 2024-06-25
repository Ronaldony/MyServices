using Microsoft.Extensions.Logging;

namespace ServerEngine.Database.Services
{
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Database.Types;

    public class BlobDatabaseService : IBlobDatabaseService
    {
        private readonly ILogger<BlobDatabaseService> _logger;

        private Dictionary<Type_BlobData, BlobDatabaseInfo> _blobDatabaseInfos;
        public Dictionary<Type_BlobData, BlobDatabaseInfo> GetBlobDatabaseInfos() => _blobDatabaseInfos;

        public BlobDatabaseService(ILogger<BlobDatabaseService> logger)
        {
            _logger = logger;

            // Map Type_BlobData with the class which BlobDataAttribute has.
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {

        }
    }
}
