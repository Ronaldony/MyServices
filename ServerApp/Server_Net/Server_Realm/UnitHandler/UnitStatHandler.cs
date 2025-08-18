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
    public class UnitStatHandler : UnitHandlerBase
    {
        private ILogger<UnitStatHandler> _logger;

        // Unit.
        private Unit _owner;

        // DTO.
        private DTO_Stat _stat;

        // 상태 업데이트 여부.
        private bool _isStatUpdate;

        public int GetPower() => _stat.Power;
        public DTO_Stat GetStat() => _stat;

        public UnitStatHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitStatHandler>>();
        }

        /// <summary>
        /// 기본 상태 초기화.
        /// </summary>
        public void Setup(Unit owner, DTO_Stat stat)
        {
            _owner = owner;
            _stat = stat;

            _isStatUpdate = false;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(double deltaTime)
        {
            // 유닛 죽음 판정 확인.
            if (_owner.IsDead) return;
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

            // Damage.
            _stat.Health -= (damage - _stat.Defense);

            if (_stat.Health <= 0)
            {
                // 유닛 죽음.
                _owner.AIBrain?.ChangeState(Type_AIState.Dead);
                _owner.IsDead = true;
            }

            // 상태 업데이트.
            _isStatUpdate = true;
        }

        #region common
        /// <summary>
        /// Refresh status.
        /// </summary>
        public bool IsStatUpdated()
        {
            var oldStatus = _isStatUpdate;
            _isStatUpdate = false;
            return oldStatus;
        }

        /// <summary>
        /// Refresh HP.
        /// </summary>
        public void RefreshHP()
        {
            _stat.Health = _stat.HealthMax;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
        #endregion
    }
}
