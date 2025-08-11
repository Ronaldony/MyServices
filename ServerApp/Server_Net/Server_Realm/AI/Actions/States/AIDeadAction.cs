
namespace Server_Realm.AI.Actions.States
{
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIDeadAction.
    /// </summary>
    public class AIDeadAction : AIAction
    {
        public AIDeadAction(MobUnit owner, AIBrain brain)
            :base(owner, brain)
        {

        }

        public override void Start()
        {
            Owner.IsDead = true;
        }

        public override void Update(double deltaTime)
        {
        }

        public override void Stop()
        {
        }
    }
}
