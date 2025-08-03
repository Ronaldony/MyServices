
namespace Server_Realm.AI.Actions
{
    using Server_Realm.AI.Actions.Interfaces;
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// AIAction.
    /// </summary>
    public abstract class AIAction : IAIAction
    {
        public AIAction(MobUnit owner)
        {
            Owner = owner;
            Brain = owner.AIBrain;
        }

        /// <summary>
        /// Unit.
        /// </summary>
        protected MobUnit Owner { get; }

        /// <summary>
        /// Need to update.
        /// </summary>
        public bool StatusUpdated { get; set; }

        /// <summary>
        /// AI brain.
        /// </summary>
        public AIBrain Brain { get; protected set; }

        /// <summary>
        /// Start.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Update.
        /// </summary>
        public abstract void Update(double deltaTime);

        /// <summary>
        /// Stop.
        /// </summary>
        public abstract void Stop();
    }
}
