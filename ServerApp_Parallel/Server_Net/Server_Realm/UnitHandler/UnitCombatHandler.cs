using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using GameCore.Datas.Plays;
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
        private readonly DTO_Stat _stat;

        public UnitCombatHandler(IServiceProvider serviceProvider, Unit owner, DTO_Stat stat)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitCombatHandler>>();

            _owner = owner;
            _stat = stat;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(double deltaTime)
        {

        }

        /// <summary>
        /// 피격.
        /// </summary>
        public void Hit(IHitbox hitbox, int damage)
        {
            _owner.UnitUpdated = true;

            // Hit box 충돌 검사.
            if (true == hitbox.IsHit(_owner.UnitCollision.GetHitbox()))
            {
                _stat.Health -= (damage - _stat.Defense);

                if (_stat.Health <= 0)
                {
                    // 죽음 상태 전환.
                    _owner?.AIBrain.ChangeState(Type_AIState.Dead);
                }
            }
        }
    }
}
