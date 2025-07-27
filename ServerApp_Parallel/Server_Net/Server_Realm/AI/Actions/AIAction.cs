
namespace Server_Realm.AI.Actions
{
    using Server_Realm.AI.Actions.Interfaces;
    using Server_Realm.Entities;

    /// <summary>
    /// AIAction.
    /// </summary>
    public abstract class AIAction : IAIAction
    {
        public AIAction(Unit owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Unit.
        /// </summary>
        protected Unit Owner { get; }

        /// <summary>
        /// Need to update.
        /// </summary>
        public bool NeedsUpdate { get; set; }

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

        /// <summary>
        /// Reset.
        /// </summary>
        public abstract void Reset();
    }
}
