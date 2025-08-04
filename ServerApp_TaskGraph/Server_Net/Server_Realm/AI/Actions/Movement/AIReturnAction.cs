using System.Numerics;

namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIReturnAction.
    /// </summary>
    public class AIReturnAction : AIAction
    {
        public AIReturnAction(MobUnit owner) 
            : base(owner)
        {
        }

        public override void Start()
        {
            // 스폰 위치로 이동 위치 설정.
            Owner.MovementHandler.MoveStart(Vector2.Normalize(Owner.MovementHandler.NowPos() - Owner.SpawnPos));
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
