using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObject
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using Framwork.Core.Session;

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
    }
}
