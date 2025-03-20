
namespace Server_World.Actors.HuntZone.Message
{
    using GameCore.Datas.Player;

    /// <summary>
    /// HuntZoneMsg_LoadPlayer.
    /// </summary>
    public record HuntZoneMsg_LoadPlayer
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayerInfo PlayInfo { get; init; }
    }
}
