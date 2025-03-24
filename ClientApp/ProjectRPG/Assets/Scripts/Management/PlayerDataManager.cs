using GameCore.Datas.Player;
using System;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    // Auth.
    public string AccessToken;
    public string RefreshToken;
    public DateTime RefrshTokenExpire;

    // Player.
    public string UserId;
    public string Uid;
    public string Pid;
    public string PlayerName;

    // Infos.
    public DTO_PlayerInfo PlayerInfo;

    /// <summary>
    /// Set authentication process.
    /// </summary>
    public void SetAuthProcess(string token, string refreshToken, DateTime tokenExpire)
    {
        AccessToken = token;
        RefreshToken = refreshToken;
        RefrshTokenExpire = tokenExpire;
    }

    /// <summary>
    /// Set player.
    /// </summary>
    public void SetPlayer(string userId = default, string uid = default, string pid = default, string playerName = default)
    {
        UserId = userId;
        Uid = uid;
        Pid = pid;
        PlayerName = playerName;
    }

    /// <summary>
    /// Set player.
    /// </summary>
    public void SetPlayerInfo(DTO_PlayerInfo playerInfo)
    {
        PlayerInfo = playerInfo;
    }
}
