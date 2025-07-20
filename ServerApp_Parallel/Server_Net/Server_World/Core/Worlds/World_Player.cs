namespace Server_World.Core.Worlds
{
    using Framework.Core.Session;
    using GameCore.Datas.Play;
    using Server_World.Core.Collisions;

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

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Section.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// OldSection.
        /// </summary>
        public SectionId OldSection { get; set; }

        /// <summary>
        /// Collision box.
        /// </summary>
        public Collision Collision { get; set; }
    }
}
