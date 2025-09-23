namespace Server_Realm.Context.AI.Actions.States
{
    using GameCore;
    using Server_Realm.Context.AI;
    using Server_Realm.Context.AI.Actions;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIIdleAction.
    /// </summary>
    public class AIIdleAction : AIAction
    {
        private double _idleTimeSec;       // Idle 상태 시간.

        public AIIdleAction(MobUnit owner, AIBrain brain)
            : base(owner, brain)
        {
        }

        public override void Start()
        {
            // Move stop.
            Owner.MovementHandler.MoveStop();

            // Clear aggro points.
            Owner.CombatHandler.ClearAggroPoints();
        }

        public override void Update(double deltaTime)
        {
            // 플레이어 감지.
            if (Owner.IsDetectPlayer(Owner.UnitId))
            {
                // idle -> Combat.
                Brain.ChangeState(Type_AIState.Combat);
                return;
            }

            // Idle time.
            _idleTimeSec += deltaTime;

            // Idle 지속 시간을 초과 시, 로밍 상태 진입.
            if (_idleTimeSec >= GameData.MOB_IDLE_TIME)
            {
                // Idle -> Roam.
                Brain.ChangeState(Type_AIState.Roam);
                return;
            }
        }

        public override void Stop()
        {
            // Idle 시간 초기화.
            _idleTimeSec = 0;
        }
    }
}
