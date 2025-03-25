using GameCore.Datas.Play;
using System.Collections.Generic;

public class AreaDataManager : Singleton<AreaDataManager>
{
    private List<DTO_PlayInfo> _playInfos;
    //private List<DTO_PlayInfo> _enemyInfos;

    public void SetPlayInfos(List<DTO_PlayInfo> playInfos)
    {
        _playInfos = playInfos;
    }
}
