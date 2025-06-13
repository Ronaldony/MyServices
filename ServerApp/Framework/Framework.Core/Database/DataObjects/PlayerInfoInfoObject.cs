using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Database.DataObject
{
    using Framework.Core.Database.Attributes;
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.Interfaces;
    using GameCore.Datas.Player;

    /// <summary>
    /// SessionInfoObject.
    /// </summary>
    [DataInfoObject(typeof(DTO_PlayerInfo))]
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

        public async Task UpsertAsync(string pid)
        {
            await _dbPlayerInfo.UpsertAsync<DTO_PlayerInfo>(pid, _playerInfo);
        }
    }
}
