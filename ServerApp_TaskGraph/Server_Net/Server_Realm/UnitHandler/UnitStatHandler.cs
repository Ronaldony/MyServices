using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using GameCore.Datas.Plays;
    using Server_Realm.Entities;

    /// <summary>
    /// UnitStatHandler.
    /// </summary>
    public class UnitStatHandler
    {
        private readonly ILogger<UnitStatHandler> _logger;

        // Unit.
        private readonly Unit _owner;

        // DTO.
        private readonly DTO_Stat _stat;

        // 상태 업데이트 여부.
        private bool _needsStatusUpdate;

        public int GetPower() => _stat.Power;
        public DTO_Stat GetStat() => _stat;

        public UnitStatHandler(IServiceProvider serviceProvider, Unit owner, DTO_Stat stat)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitStatHandler>>();

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
        /// Update.
        /// </summary>
        public void Damage(int damage)
        {
            if (_owner.IsDead)
            {
                return;
            }

            _stat.Health -= (damage - _stat.Defense);

            if (_stat.Health <= 0)
            {
                // 유닛 죽음.
                _owner?.AIBrain.ChangeState(Type_AIState.Dead);
                _owner.IsDead = true;
            }

            // 상태 업데이트.
            _needsStatusUpdate = true;
        }

        #region common
        /// <summary>
        /// Refresh status.
        /// </summary>
        public bool NeedsStatusUpdated()
        {
            var oldStatus = _needsStatusUpdate;
            _needsStatusUpdate = false;
            return oldStatus;
        }

        /// <summary>
        /// Refresh HP.
        /// </summary>
        public void RefreshHP()
        {
            _stat.Health = _stat.HealthMax;
        }
        #endregion
    }
}
