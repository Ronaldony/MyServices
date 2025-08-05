using System.Numerics;

namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AICombatAction.
    /// </summary>
    public class AICombatAction : AIAction
    {
        private readonly int _attackCooldown;       // 공격 쿨타임.
        private double _nowCooldown;                   // 현재 누적 쿨타임.

        private readonly int _attackRange;          // 공격 사거리.
        private readonly int _attackRangeSqr;       // 공격 사거리 제곱근. (거리 비교용)

        // Target.
        private string _targetPid;
        private Vector2 _targetPos;

        public AICombatAction(MobUnit owner, TB_Mob tbMob)
            : base(owner)
        {
            // Setup.
            _attackCooldown = tbMob.AttackCooldown;
            _attackRange = tbMob.AttackRange;
            _attackRangeSqr = _attackRange * _attackRange;
        }

        public override void Start()
        {
            var detectPlayer = Owner.DetectPlayer(Owner.UnitId);

            // 타겟이 없음.
            if (null == detectPlayer)
            {
                // Combat -> idle.
                Brain.ChangeState(Type_AIState.Idle);
                return;
            }

            // Set position.
            _targetPid = detectPlayer.Pid;
            _targetPos = detectPlayer.MovementHandler.NowPos();

            // Move start.
            Owner.MovementHandler.MoveStart(Vector2.Normalize(Owner.MovementHandler.NowPos() - _targetPos));
        }

        public override void Update(double deltaTime)
        {
            var target = Owner.ZoneProcessor.GetPlayer(_targetPid);
            
            // 타겟이 Zone에 없거나 죽은 경우 Idle 상태 진입.
            if (null == target || target.IsDead)
            {
                // Combat -> Idle.
                Brain.ChangeState(Type_AIState.Idle);
                return;
            }

            _nowCooldown += deltaTime;

            var nowTargetPos = target.MovementHandler.NowPos();
            var ownerPos = Owner.MovementHandler.NowPos();

            // 타겟이 공격 사정 거리인지 확인.
            if (Vector2.DistanceSquared(ownerPos, nowTargetPos) <= _attackRangeSqr)
            {
                // 공격 쿨타임 검사.
                if (_nowCooldown >= _attackCooldown)
                {
                    // 쿨타임 초기화.
                    _nowCooldown = 0;

                    // 공격.
                    Owner.Attack(target);
                }

                return;
            }

            // 현재 위치가 스폰 위치로부터 벗어났는지 확인.
            if (Vector2.DistanceSquared(Owner.SpawnPos, ownerPos) <= GameData.MOB_RETURN_DISTANCE_SQUARE)
            {
                // Combat -> Return.
                Brain.ChangeState(Type_AIState.Return);
                return;
            }

            // 타겟의 위치가 변경된 경우, 추적 위치 갱신.
            if (nowTargetPos != _targetPos)
            {
                // 추적 위치 갱신.
                _targetPos = nowTargetPos;
                Owner.MovementHandler.MoveStart(Vector2.Normalize(ownerPos - nowTargetPos));
            }
        }

        public override void Stop()
        {
            // 초기화.
            _targetPid = string.Empty;
            _nowCooldown = 0;
        }
    }
}
