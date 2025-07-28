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

        // 상태.
        private bool _statusChaned;

        #region common
        public int GetPower() => _stat.Power;
        public DTO_Stat GetStat() => _stat;
        #endregion

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
            if (!_owner.IsActive)
            {
                return;
            }

            // 스탯 상태 변경.
            _statusChaned = true;

            _stat.Health -= (damage - _stat.Defense);

            if (_stat.Health <= 0)
            {
                // 죽음 상태 전환.
                _owner?.AIBrain.ChangeState(Type_AIState.Dead);

                // Unit 비활성화.
                _owner.IsActive = false;
            }
        }

        #region common
        /// <summary>
        /// Refresh status.
        /// </summary>
        public bool RefreshStatus()
        {
            var oldStatus = _statusChaned;
            _statusChaned = false;
            return oldStatus;
        }
        #endregion
    }
}
