using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Database.Interfaces;

namespace Framework.Core.Database.DataObjects
{
    using Consul;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Player;
    using ServerEngine.Database.DBObject.Transactions;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class PlayerInfoObject : IDataObject
    {
        public DTO_PlayerInfo PlayerInfo => _playerInfo;

        private DTO_PlayerInfo _playerInfo;
        private readonly DBPlayerInfo _dbPlayerInfo;

        public PlayerInfoObject(IServiceProvider serviceProvider)
        {
            _dbPlayerInfo = serviceProvider.GetRequiredService<DBPlayerInfo>();
        }

        public async Task LoadDataAsync(string pid)
        {
            _playerInfo = await _dbPlayerInfo.SelectAsync(pid);

            if (null == _playerInfo)
            {
                _playerInfo = new DTO_PlayerInfo();
            }
        }

        public async Task<bool> UpsertAsync(string pid)
        {
            return await _dbPlayerInfo.UpsertAsync<DTO_PlayerInfo>(pid, _playerInfo);
        }

        /// <summary>
        /// Get DB object.
        /// </summary>
        public IDBObject GetDBObject()
        {
            return _dbPlayerInfo;
        }

        /// <summary>
        /// Upsert async for transaction.
        /// </summary>
        public async Task<bool> UpsertAsync(TransactionContext tranContext, string pid)
        {
            return await tranContext.UpsertAsync(_dbPlayerInfo, pid, _playerInfo);
        }
    }
}
