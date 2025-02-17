namespace ServerEngine.Core.Services.Interfaces
{
    public interface IUniqueIdService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize(DateTime baseTime, int dcId, int serverId);

        /// <summary>
        /// Generate primary key.
        /// </summary>
        string GenerateId();
    }
}
