using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Database.Interfaces;

namespace Framework.Core.Database.DataObjects
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using Framwork.Core.Session;
    using ServerEngine.Database.DBObject.Transactions;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class SessionInfoObject : IDataObject
    {
        public SessionInfo SessionInfo => _sessionInfo;

        private SessionInfo _sessionInfo;
        private readonly DBSessionInfo _dbSessionInfo;

        public SessionInfoObject(IServiceProvider serviceProvider)
        {
            _dbSessionInfo = serviceProvider.GetRequiredService<DBSessionInfo>();
        }

        /// <summary>
        /// Load data.
        /// </summary>
        public async Task LoadDataAsync(string pid)
        {
            _sessionInfo = await _dbSessionInfo.SelectAsync(pid);

            if (null == _sessionInfo)
            {
                _sessionInfo = new SessionInfo();
            }
        }

        /// <summary>
        /// Load data.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid)
        {
            return await _dbSessionInfo.UpsertAsync(pid, _sessionInfo);
        }

        /// <summary>
        /// Get DB object.
        /// </summary>
        public IDBObject GetDBObject()
        {
            return _dbSessionInfo;
        }

        /// <summary>
        /// Upsert async for transaction.
        /// </summary>
        public async Task<bool> UpsertAsync(TransactionContext tranContext, string pid)
        {
            return await tranContext.UpsertAsync(_dbSessionInfo, pid, _sessionInfo);
        }
    }
}
