using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObjects
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Player;
    using ServerEngine.Database.DBObject.Transactions;
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    public class PostInfoObject : IDataObject
    {
        public DTO_PostInfo PostInfo => _postInfo;

        private DTO_PostInfo _postInfo;
        private readonly DBPostInfo _dbPostInfo;

        public PostInfoObject(IServiceProvider serviceProvider)
        {
            _dbPostInfo = serviceProvider.GetRequiredService<DBPostInfo>();
        }

        public async Task LoadDataAsync(string pid)
        {
            _postInfo = await _dbPostInfo.SelectAsync(pid);

            if (null == _postInfo)
            {
                _postInfo = new DTO_PostInfo();
            }
        }

        public async Task<bool> UpsertAsync(string pid)
        {
            return await _dbPostInfo.UpsertAsync<DTO_PostInfo>(pid, _postInfo);
        }

        /// <summary>
        /// Get DB object.
        /// </summary>
        public IDBObject GetDBObject()
        {
            return _dbPostInfo;
        }

        /// <summary>
        /// Upsert async for transaction.
        /// </summary>
        public async Task<bool> UpsertAsync(TransactionContext tranContext, string pid)
        {
            return await tranContext.UpsertAsync(_dbPostInfo, pid, _postInfo);
        }
    }
}
