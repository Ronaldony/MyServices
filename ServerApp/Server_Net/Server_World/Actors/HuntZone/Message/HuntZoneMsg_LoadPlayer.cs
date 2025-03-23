
namespace Server_World.Actors.HuntZone.Message
{
    using GameCore.Datas.Play;

    /// <summary>
    /// HuntZoneMsg_LoadPlayer.
    /// </summary>
    public record HuntZoneMsg_LoadPlayer
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; set; }
    }
}
