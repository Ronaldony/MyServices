using System.Numerics;

namespace Server_Realm.Context.AI.Actions.Movement
{
    using Framework.Core.Utilities;
    using GameCore;
    using Server_Realm.Context.AI;
    using Server_Realm.Context.AI.Actions;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIReturnAction.
    /// </summary>
    public class AIReturnAction : AIAction
    {
        public AIReturnAction(MobUnit owner, AIBrain brain)
            : base(owner, brain)
        {
        }

        public override void Start()
        {
            // 스폰 위치로 이동 위치 설정.
            Owner.MovementHandler.MoveStart(VectorUtility.SafeNormalize(Owner.SpawnPos, Owner.MovementHandler.NowPos()));

            // Aggro 초기화.
            Owner.CombatHandler.ClearAggroPoints();
        }

        public override void Update(double deltaTime)
        {
            // Refresh HP.
            Owner.StatHandler.RefreshHP();

            // 스폰 위치 확인.
            if (Vector2.DistanceSquared(Owner.MovementHandler.NowPos(), Owner.SpawnPos) <= GameData.MOB_ARRIVAL_RANGE_SQUARE)
            {
                // Return -> Idle.
                Brain.ChangeState(Type_AIState.Idle);
                return;
            }
        }

        public override void Stop()
        {

        }
    }
}
