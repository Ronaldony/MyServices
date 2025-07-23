
namespace Server_World.Core.Worlds
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Collisions;
    using System.Numerics;

    /// <summary>
    /// Wolrd_enemy.
    /// </summary>
    public class World_Enemy
    {
        public World_Enemy()
        {

        }

        /// <summary>
        /// DTO_Enemy.
        /// </summary>
        public DTO_Enemy Enemy { get; set; }

        /// <summary>
        /// Position.
        /// DTO_Enemy 클래스의 Location Position과 동일한 위치.
        /// 외부로 노출시켜 접근 비용을 줄이려는 목적.
        /// </summary>
        public Vector2 Position { get; set; }

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

        public void Update_Move()
        {

        }
    }
}
