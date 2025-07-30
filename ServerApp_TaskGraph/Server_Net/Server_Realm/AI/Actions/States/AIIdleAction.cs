namespace Server_Realm.AI.Actions.States
{
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// AIIdleAction.
    /// </summary>
    public class AIIdleAction : AIAction
    {
        public AIIdleAction(MobUnit owner)
            : base(owner)
        {
        }
        public override void Start()
        {
            // Move stop.
            Owner.MovementHandler.MoveStop();
        }

        public override void Update(double deltaTime)
        {
        }

        public override void Stop()
        {
        }

    }
}
