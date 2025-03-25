
namespace Server_World.Actors.HuntZone.Message
{
    using Framwork.Core.Session;
    using GameCore.Datas.Play;

    /// <summary>
    /// HuntZoneMsg_LoadPlayer.
    /// </summary>
    public record HuntZoneMsg_LoadPlayer
    {
        /// <summary>
        /// GameSession.
        /// </summary>
        public GameSession GameSession { get; set; }

        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; set; }
    }
}
