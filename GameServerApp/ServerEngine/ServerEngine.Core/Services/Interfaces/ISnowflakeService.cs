namespace ServerEngine.Core.Services.Interfaces
{
    public interface ISnowflakeService
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
