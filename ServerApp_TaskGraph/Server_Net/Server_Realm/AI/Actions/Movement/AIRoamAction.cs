using System.Numerics;

namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using GameCore.Datas.Plays;
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

        public AIRoamAction(MobUnit owner)
            : base(owner)
        {
            _location = owner.Mob.Location;
            _rand = new Random();
        }

        public override void Start()
        { 
            // 로밍 위치 설정.
            _roamPos = new Vector2(
                x: Owner.Mob.Location.Pos.X + _rand.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT),
                y: Owner.Mob.Location.Pos.Y + _rand.Next(-GameData.MOVEMENT_RANGE_LIMIT, GameData.MOVEMENT_RANGE_LIMIT));

            // 로밍 방향 갱신.
            Owner.MovementHandler.MoveStart(Vector2.Normalize(_location.Pos - _roamPos));
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
