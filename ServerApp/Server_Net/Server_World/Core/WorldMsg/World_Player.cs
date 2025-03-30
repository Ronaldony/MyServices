namespace Server_World.Core.WorldMsg
{
    using Framwork.Core.Session;
    using GameCore.Datas.Play;

    /// <summary>
    /// World_Player.
    /// </summary>
    public class World_Player
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
