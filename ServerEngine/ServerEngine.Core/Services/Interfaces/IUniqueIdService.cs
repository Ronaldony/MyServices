namespace ServerEngine.Core.Services.Interfaces
{
    public interface IUniqueIdService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize(DateTime baseTime, int dcId, int serverId);

        /// <summary>
        /// Generate id as string.
        /// </summary>
        string GenerateId();

        /// <summary>
        /// Generate id as long.
        /// </summary>
        long GenerateIdAsLong();
    }
}
