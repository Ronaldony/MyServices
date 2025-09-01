using System.Numerics;

namespace Server_Realm.Context.AI.Actions.Movement
{
    using GameCore;
    using GameCore.Datas.Plays;
    using Server_Realm.Context.AI;
    using Server_Realm.Context.AI.Actions;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIRoamAction.
    /// </summary>
    public class AIRoamAction : AIAction
    {
        private readonly DTO_Location _location;

        // 로밍 위치.
        private Vector2 _roamPos;

        // 랜덤.
        private Random _rand;

        public AIRoamAction(MobUnit owner, AIBrain brain)
            : base(owner, brain)
        {
            _location = owner.Mob.Location;
            _rand = new Random();
        }

        public override void Start()
        {
            // 로밍 위치 설정.
            _roamPos = new Vector2(
                x: Owner.SpawnPos.X + _rand.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT),
                y: Owner.SpawnPos.Y + _rand.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT));

            // 로밍 방향 갱신.
            Owner.MovementHandler.MoveStart(VectorUtility.SafeNormalize(_roamPos, _location.Pos));
        }

        public override void Update(double deltaTime)
        {
            // 플레이어 감지.
            if (Owner.IsDetectPlayer(Owner.UnitId))
            {
                // Roam -> Combat.
                Brain.ChangeState(Type_AIState.Combat);
                return;
            }

            // 로밍 위치에 도착하였는지 검사.
            if (Vector2.DistanceSquared(_roamPos, _location.Pos) <= GameData.MOB_ARRIVAL_RANGE)
            {
                // Roam -> Idle.
                Brain.ChangeState(Type_AIState.Idle);
                return;
            }
        }

        public override void Stop()
        {
        }
    }
}
