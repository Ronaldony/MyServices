using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObjects
{
    using Consul;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Play;
    using ServerEngine.Database.DBObject.Transactions;
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// PlayInfoObject.
    /// </summary>
    public class PlayInfoObject : IDataObject
    {
        public DTO_PlayInfo PlayInfo => _playInfo;

        private DTO_PlayInfo _playInfo;
        private readonly DBPlayInfo _dbPlayInfo;

        public PlayInfoObject(IServiceProvider serviceProvider)
        {
            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
        }

        public async Task LoadDataAsync(string pid)
        {
            _playInfo = await _dbPlayInfo.SelectAsync(pid);

            if (null == _playInfo)
            {
                _playInfo = new DTO_PlayInfo();
            }
        }

        public async Task<bool> UpsertAsync(string pid)
        {
            return await _dbPlayInfo.UpsertAsync<DTO_PlayInfo>(pid, _playInfo);
        }

        /// <summary>
        /// Get DB object.
        /// </summary>
        public IDBObject GetDBObject()
        {
            return _dbPlayInfo;
        }

        /// <summary>
        /// Upsert async for transaction.
        /// </summary>
        public async Task<bool> UpsertAsync(TransactionContext tranContext, string pid)
        {
            return await tranContext.UpsertAsync(_dbPlayInfo, pid, _playInfo);
        }
    }
}
