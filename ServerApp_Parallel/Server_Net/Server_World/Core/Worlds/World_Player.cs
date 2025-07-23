namespace Server_World.Core.Worlds
{
    using Framework.Core.Session;
    using GameCore.Datas.Play;
    using Server_World.Core.Collisions;
    using System.Numerics;

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
        /// Position.
        /// DTO_PlayInfo 클래스의 Location Position과 동일한 위치.
        /// 외부로 노출시켜 접근 비용을 줄이려는 목적.
        /// </summary>
        public Vector2 Position { get; set; }

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
