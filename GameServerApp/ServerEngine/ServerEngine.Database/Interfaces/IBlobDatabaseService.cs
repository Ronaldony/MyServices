using ServerEngine.Database.Types;

namespace ServerEngine.Database.Interfaces
{
    public interface IBlobDatabaseService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get blob database infos.
        /// </summary>
        Dictionary<Type_BlobData, BlobDatabaseInfo> GetBlobDatabaseInfos();
    }
}
