using GameCore.Datas.Inventory;
using GameCore.Datas.Play;
using GameCore.Datas.Player;
using System;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    // Auth.
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefrshTokenExpire { get; set; }

    // Player.
    public string UserId { get; set; }
    public string Uid { get; set; }
    public string Pid { get; set; }
    public string PlayerName { get; set; }

    // Unit.
    public long UnitId { get; set; }

    // Infos.
    public DTO_PlayerInfo PlayerInfo { get; set; }
    public DTO_InventoryInfo InventoryInfo { get; set; }
    public DTO_PlayInfo PlayInfo { get; set; }

    /// <summary>
    /// Set authentication process.
    /// </summary>
    public void SetAccessToken(string token, string refreshToken, DateTime tokenExpire)
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
}
