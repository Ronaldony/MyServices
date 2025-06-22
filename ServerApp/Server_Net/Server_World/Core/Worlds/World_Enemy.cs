
namespace Server_World.Core.Worlds
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Collision;

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
        /// Collision box.
        /// </summary>
        public CollisionBox CollisionBox { get; set; }
    }
}
