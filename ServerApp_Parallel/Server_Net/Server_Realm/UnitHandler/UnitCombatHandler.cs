using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.UnitHandler
{
    using GameCore.Datas.Plays;
    using Server_Realm.AI;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;

    /// <summary>
    /// UnitCombatHandler.
    /// </summary>
    public class UnitCombatHandler
    {
        private readonly ILogger<UnitCombatHandler> _logger;

        // Unit.
        private readonly Unit _owner;

        // DTO.
        private readonly DTO_Location _location;
        private readonly DTO_Stat _stat;

        public UnitCombatHandler(IServiceProvider serviceProvider, Unit owner, DTO_Location location, DTO_Stat stat)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitCombatHandler>>();

            _owner = owner;
            _location = location;
            _stat = stat;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(float deltaTime)
        {

        }

        /// <summary>
        /// 근접 공격.
        /// </summary>
        public bool Attack(IHitbox hitbox, int damage)
        {
            // Hit box 충돌 검사.
            if (true == hitbox.IsHit(_owner.UnitCollision.GetHitbox()))
            {
                _stat.Health -= damage;

                if (_stat.Health <= 0)
                {
                    // 죽음 상태 전환.
                    _owner?.AIBrain.UpdateAction
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hit()
        {

        }
    }
}
