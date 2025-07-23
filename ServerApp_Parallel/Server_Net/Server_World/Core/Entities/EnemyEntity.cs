using System.Numerics;

namespace Server_World.Core.Entities
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Collisions;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// EnemyEntity.
    /// </summary>
    public class EnemyEntity : IGameEntity
    {
        public EnemyEntity()
        {
            EntityType = Type_Entity.Enemy;
        }

        /// <summary>
        /// 엔티티 활성 여부.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Type_Entity.
        /// </summary>
        public Type_Entity EntityType { get; init; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// 현재 섹션.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// 이전 섹션.
        /// </summary>
        public SectionId OldSection { get; set; }

        /// <summary>
        /// Collision.
        /// </summary>
        public Collision Collision { get; set; }

        /// <summary>
        /// DTO_Enemy.
        /// </summary>
        public DTO_Enemy Enemy { get; set; }

        /// <summary>
        /// Enemy uid.
        /// </summary>
        public ulong EnemyUid { get; set; }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
