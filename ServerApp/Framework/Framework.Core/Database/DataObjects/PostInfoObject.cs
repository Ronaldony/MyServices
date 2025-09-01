using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObjects
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Player;

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
    }
}
