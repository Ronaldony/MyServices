using System.Numerics;

namespace Server_World.Core.Entities
{
    using GameCore.Datas.Play;
    using Server_World.Core.Collisions;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// Player Entity.
    /// </summary>
    public class PlayerEntity : IGameEntity
    {
        public PlayerEntity()
        {
            EntityType = Type_Entity.Player;
        }

        /// <summary>
        /// 활성 여부
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
        /// OldSection.
        /// </summary>
        public SectionId OldSection { get; set; }

        /// <summary>
        /// Collision.
        /// </summary>
        public Collision Collision { get; set; }

        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

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
