
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.Core.Database.DataObject
{
    using Framwork.Core.Session;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Database.DataObject;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class SessionInfoObject : DataObject_Couchbase
    {
        private readonly ILogger<SessionInfoObject> _logger;

        public SessionInfoObject(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager._configGame.Couchbase,
                  GameConfigManager.GetCouchbaseConnection("SessionInfo"))
        {
            _logger = serviceProvider.GetRequiredService<ILogger<SessionInfoObject>>();
        }

        /// <summary>
        /// Select async.
        /// </summary>s
        public new async Task<T> SelectAsync<T>(string pid) where T : SessionInfo, new()
        {
            var playerInfo = await base.SelectAsync<T>(pid);

            if (null == playerInfo)
            {
                _logger.LogError("failed to select GameSession.");
                return null;
            }

            return playerInfo;
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public new async Task<bool> UpsertAsync<T>(string pid, T dataObject) where T : SessionInfo, new()
        {
            var isUpsert = await base.UpsertAsync(pid, dataObject);

            if (false == isUpsert)
            {
                _logger.LogError("failed to upsert GameSession.");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Create first object.
        /// </summary>
        protected override T FirstCreateObject<T>()
        {
            throw new NotImplementedException();
        }
    }
}
