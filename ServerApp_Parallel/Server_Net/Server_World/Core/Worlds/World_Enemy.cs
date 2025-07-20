
namespace Server_World.Core.Worlds
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Collisions;

    /// <summary>
    /// Wolrd_enemy.
    /// </summary>
    public class World_Enemy
    {
        /// <summary>
        /// DTO_Enemy.
        /// </summary>
        public DTO_Enemy Enemy { get; set; }

        /// <summary>
        /// Section.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// Enemy uid.
        /// </summary>
        public ulong EnemyUid { get; set; }

        /// <summary>
        /// Collision box.
        /// </summary>
        public Collision CollisionBox { get; set; }
    }
}
