namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// AICombatAction.
    /// </summary>
    public class AICombatAction : AIAction
    {
        public AICombatAction(MobUnit owner)
            : base(owner)
        {
        }

        public override void Start()
        {
        }

        public override void Update(double deltaTime)
        {

        }

        public override void Stop()
        {
            // TODO: 표적 제거.
            
            // AIReturn 상태 변경.
            Owner.AIBrain.ChangeState(Type_AIState.Return);
        }
    }
}
